using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class InventarioViewModel : ObservableObject
{
    private readonly ProductoService _productoService;
    private readonly InventarioService _inventarioService;
    private int _usuarioId;

    public ObservableCollection<Producto> ResultadosBusqueda { get; } = new();
    public ObservableCollection<MovimientoKardex> Kardex { get; } = new();
    public IEnumerable<TipoAjusteInventario> TiposAjuste => Enum.GetValues<TipoAjusteInventario>();

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private Producto? productoSeleccionado;

    [ObservableProperty]
    private TipoAjusteInventario tipoAjuste = TipoAjusteInventario.Entrada;

    [ObservableProperty]
    private int cantidadAjuste;

    [ObservableProperty]
    private string motivoAjuste = string.Empty;

    [ObservableProperty]
    private string mensaje = string.Empty;

    public InventarioViewModel(ProductoService productoService, InventarioService inventarioService)
    {
        _productoService = productoService;
        _inventarioService = inventarioService;
    }

    public void EstablecerUsuario(int usuarioId) => _usuarioId = usuarioId;

    [RelayCommand]
    private async Task BuscarAsync()
    {
        ResultadosBusqueda.Clear();
        foreach (var producto in await _productoService.ObtenerTodosAsync(TextoBusqueda))
        {
            ResultadosBusqueda.Add(producto);
        }
    }

    partial void OnProductoSeleccionadoChanged(Producto? value)
    {
        Mensaje = string.Empty;
        _ = CargarKardexAsync();
    }

    private async Task CargarKardexAsync()
    {
        Kardex.Clear();
        if (ProductoSeleccionado is null)
        {
            return;
        }

        foreach (var movimiento in await _inventarioService.ObtenerKardexAsync(ProductoSeleccionado.Id))
        {
            Kardex.Add(movimiento);
        }
    }

    [RelayCommand]
    private async Task RegistrarAjusteAsync()
    {
        if (ProductoSeleccionado is null)
        {
            Mensaje = "Seleccione un producto.";
            return;
        }

        if (string.IsNullOrWhiteSpace(MotivoAjuste))
        {
            Mensaje = "Ingrese el motivo del ajuste.";
            return;
        }

        var error = await _inventarioService.RegistrarAjusteAsync(ProductoSeleccionado.Id, TipoAjuste, CantidadAjuste,
            MotivoAjuste, _usuarioId);

        if (error is not null)
        {
            Mensaje = error;
            return;
        }

        Mensaje = "Ajuste registrado correctamente.";
        CantidadAjuste = 0;
        MotivoAjuste = string.Empty;
        await BuscarAsync();
        await CargarKardexAsync();
    }
}
