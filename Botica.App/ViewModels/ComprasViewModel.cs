using System.Collections.ObjectModel;
using System.ComponentModel;
using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class ComprasViewModel : ObservableObject
{
    private readonly ProductoService _productoService;
    private readonly ProveedorService _proveedorService;
    private readonly CompraService _compraService;
    private int _usuarioId;

    public ObservableCollection<Proveedor> ProveedoresDisponibles { get; } = new();
    public ObservableCollection<Producto> ResultadosBusqueda { get; } = new();
    public ObservableCollection<ItemCarritoCompra> Carrito { get; } = new();

    [ObservableProperty]
    private Proveedor? proveedorSeleccionado;

    [ObservableProperty]
    private string numeroComprobante = string.Empty;

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private Producto? productoSeleccionadoBusqueda;

    [ObservableProperty]
    private ItemCarritoCompra? itemSeleccionado;

    [ObservableProperty]
    private string mensaje = string.Empty;

    public decimal Total => Carrito.Sum(i => i.Subtotal);

    public ComprasViewModel(ProductoService productoService, ProveedorService proveedorService,
        CompraService compraService)
    {
        _productoService = productoService;
        _proveedorService = proveedorService;
        _compraService = compraService;
    }

    public void EstablecerUsuario(int usuarioId) => _usuarioId = usuarioId;

    [RelayCommand]
    private async Task CargarAsync()
    {
        ProveedoresDisponibles.Clear();
        foreach (var proveedor in await _proveedorService.ObtenerTodosAsync())
        {
            ProveedoresDisponibles.Add(proveedor);
        }
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        ResultadosBusqueda.Clear();
        foreach (var producto in await _productoService.ObtenerTodosAsync(TextoBusqueda))
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
        var item = new ItemCarritoCompra
        {
            ProductoId = producto.Id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            PrecioUnitario = producto.PrecioCompra,
            Cantidad = 1
        };
        item.PropertyChanged += Item_PropertyChanged;
        Carrito.Add(item);
        NotificarTotal();
    }

    [RelayCommand]
    private void QuitarDelCarrito()
    {
        if (ItemSeleccionado is null)
        {
            return;
        }

        ItemSeleccionado.PropertyChanged -= Item_PropertyChanged;
        Carrito.Remove(ItemSeleccionado);
        ItemSeleccionado = null;
        NotificarTotal();
    }

    [RelayCommand]
    private async Task ConfirmarCompraAsync()
    {
        if (ProveedorSeleccionado is null)
        {
            Mensaje = "Seleccione un proveedor.";
            return;
        }

        if (Carrito.Count == 0)
        {
            Mensaje = "Agregue al menos un producto.";
            return;
        }

        if (Carrito.Any(i => string.IsNullOrWhiteSpace(i.NumeroLote)))
        {
            Mensaje = "Todos los productos deben tener número de lote.";
            return;
        }

        var compra = new Compra
        {
            ProveedorId = ProveedorSeleccionado.Id,
            UsuarioId = _usuarioId,
            NumeroComprobante = NumeroComprobante,
            Total = Total
        };

        foreach (var item in Carrito)
        {
            compra.Detalles.Add(new DetalleCompra
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Subtotal = item.Subtotal,
                NumeroLote = item.NumeroLote,
                FechaVencimiento = item.FechaVencimiento
            });
        }

        await _compraService.RegistrarCompraAsync(compra);
        Mensaje = "Compra registrada correctamente.";
        NuevaCompra();
    }

    [RelayCommand]
    private void NuevaCompra()
    {
        foreach (var item in Carrito)
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }

        Carrito.Clear();
        NumeroComprobante = string.Empty;
        ProveedorSeleccionado = null;
        ResultadosBusqueda.Clear();
        TextoBusqueda = string.Empty;
        NotificarTotal();
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ItemCarritoCompra.Subtotal))
        {
            NotificarTotal();
        }
    }

    private void NotificarTotal() => OnPropertyChanged(nameof(Total));
}
