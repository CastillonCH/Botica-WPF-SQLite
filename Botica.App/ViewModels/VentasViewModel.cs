using System.Collections.ObjectModel;
using System.ComponentModel;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class VentasViewModel : ObservableObject
{
    private const decimal TasaIgv = 0.18m;

    private readonly ProductoService _productoService;
    private readonly VentaService _ventaService;
    private readonly CajaService _cajaService;
    private int _usuarioId;
    private Caja? _cajaActual;

    public ObservableCollection<Producto> ResultadosBusqueda { get; } = new();
    public ObservableCollection<ItemCarritoVenta> Carrito { get; } = new();
    public ObservableCollection<Venta> HistorialCajaActual { get; } = new();

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private Producto? productoSeleccionadoBusqueda;

    [ObservableProperty]
    private ItemCarritoVenta? itemSeleccionado;

    [ObservableProperty]
    private MetodoPago metodoPago = MetodoPago.Efectivo;

    [ObservableProperty]
    private decimal montoRecibido;

    [ObservableProperty]
    private string mensaje = string.Empty;

    [ObservableProperty]
    private bool hayCajaAbierta;

    [ObservableProperty]
    private Venta? ventaSeleccionadaHistorial;

    [ObservableProperty]
    private string motivoAnulacion = string.Empty;

    public IEnumerable<MetodoPago> MetodosPago => Enum.GetValues<MetodoPago>();

    public decimal Total => Carrito.Sum(i => i.Subtotal);
    public decimal Igv => Total - Total / (1 + TasaIgv);
    public decimal SubtotalSinIgv => Total - Igv;
    public decimal Vuelto => MontoRecibido - Total;

    public VentasViewModel(ProductoService productoService, VentaService ventaService, CajaService cajaService)
    {
        _productoService = productoService;
        _ventaService = ventaService;
        _cajaService = cajaService;
    }

    public void EstablecerUsuario(int usuarioId) => _usuarioId = usuarioId;

    [RelayCommand]
    private async Task CargarAsync()
    {
        _cajaActual = await _cajaService.ObtenerCajaAbiertaAsync();
        HayCajaAbierta = _cajaActual is not null;
        Mensaje = HayCajaAbierta ? string.Empty : "No hay una caja abierta. Ábrala desde el módulo de Caja.";

        HistorialCajaActual.Clear();
        if (_cajaActual is not null)
        {
            foreach (var venta in await _ventaService.ObtenerPorCajaAsync(_cajaActual.Id))
            {
                HistorialCajaActual.Add(venta);
            }
        }
    }

    [RelayCommand]
    private async Task AnularVentaAsync()
    {
        if (VentaSeleccionadaHistorial is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(MotivoAnulacion))
        {
            Mensaje = "Ingrese el motivo de la anulación.";
            return;
        }

        await _ventaService.AnularVentaAsync(VentaSeleccionadaHistorial.Id, MotivoAnulacion);
        MotivoAnulacion = string.Empty;
        Mensaje = "Venta anulada correctamente.";
        await CargarAsync();
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        ResultadosBusqueda.Clear();
        foreach (var producto in await _productoService.BuscarParaVentaAsync(TextoBusqueda))
        {
            ResultadosBusqueda.Add(producto);
        }
    }

    [RelayCommand]
    private void AgregarAlCarrito()
    {
        if (ProductoSeleccionadoBusqueda is null)
        {
            return;
        }

        var producto = ProductoSeleccionadoBusqueda;
        var existente = Carrito.FirstOrDefault(i => i.ProductoId == producto.Id);

        if (existente is not null)
        {
            existente.Cantidad += 1;
        }
        else
        {
            var item = new ItemCarritoVenta
            {
                ProductoId = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                PrecioUnitario = producto.PrecioVenta,
                StockDisponible = producto.StockActual,
                Cantidad = 1
            };
            item.PropertyChanged += ItemCarrito_PropertyChanged;
            Carrito.Add(item);
        }

        NotificarTotales();
    }

    [RelayCommand]
    private void QuitarDelCarrito()
    {
        if (ItemSeleccionado is null)
        {
            return;
        }

        ItemSeleccionado.PropertyChanged -= ItemCarrito_PropertyChanged;
        Carrito.Remove(ItemSeleccionado);
        ItemSeleccionado = null;
        NotificarTotales();
    }

    [RelayCommand]
    private async Task ConfirmarVentaAsync()
    {
        if (_cajaActual is null)
        {
            Mensaje = "No hay una caja abierta.";
            return;
        }

        if (Carrito.Count == 0)
        {
            Mensaje = "Agregue al menos un producto.";
            return;
        }

        if (MetodoPago == MetodoPago.Efectivo && MontoRecibido < Total)
        {
            Mensaje = "El monto recibido es menor al total.";
            return;
        }

        var venta = new Venta
        {
            UsuarioId = _usuarioId,
            CajaId = _cajaActual.Id,
            TipoComprobante = TipoComprobante.Ticket,
            Subtotal = SubtotalSinIgv,
            Descuento = Carrito.Sum(i => i.Descuento),
            Igv = Igv,
            Total = Total,
            MetodoPago = MetodoPago,
            MontoRecibido = MetodoPago == MetodoPago.Efectivo ? MontoRecibido : Total,
            Vuelto = MetodoPago == MetodoPago.Efectivo ? Vuelto : 0,
            Estado = EstadoVenta.Emitida
        };

        foreach (var item in Carrito)
        {
            venta.Detalles.Add(new DetalleVenta
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Descuento = item.Descuento,
                Subtotal = item.Subtotal
            });
        }

        try
        {
            await _ventaService.RegistrarVentaAsync(venta);
            Mensaje = $"Venta registrada. Vuelto: {Vuelto:N2}";
            HistorialCajaActual.Insert(0, venta);
            NuevaVenta();
        }
        catch (InvalidOperationException ex)
        {
            Mensaje = ex.Message;
        }
    }

    [RelayCommand]
    private void NuevaVenta()
    {
        foreach (var item in Carrito)
        {
            item.PropertyChanged -= ItemCarrito_PropertyChanged;
        }

        Carrito.Clear();
        MontoRecibido = 0;
        MetodoPago = MetodoPago.Efectivo;
        ResultadosBusqueda.Clear();
        TextoBusqueda = string.Empty;
        NotificarTotales();
    }

    private void ItemCarrito_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ItemCarritoVenta.Subtotal))
        {
            NotificarTotales();
        }
    }

    partial void OnMontoRecibidoChanged(decimal value) => OnPropertyChanged(nameof(Vuelto));

    private void NotificarTotales()
    {
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(Igv));
        OnPropertyChanged(nameof(SubtotalSinIgv));
        OnPropertyChanged(nameof(Vuelto));
    }
}
