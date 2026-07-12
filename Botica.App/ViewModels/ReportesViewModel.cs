using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class ReportesViewModel : ObservableObject
{
    private readonly ReportesService _service;

    public ObservableCollection<VentaPorProductoDto> VentasPorProducto { get; } = new();
    public ObservableCollection<VentaPorUsuarioDto> VentasPorUsuario { get; } = new();
    public ObservableCollection<CompraPorProveedorDto> ComprasPorProveedor { get; } = new();
    public ObservableCollection<GananciaPorDiaDto> GananciasPorDia { get; } = new();
    public ObservableCollection<Producto> StockBajo { get; } = new();
    public ObservableCollection<Producto> Agotados { get; } = new();
    public ObservableCollection<Producto> StockActual { get; } = new();

    [ObservableProperty]
    private DateTime desde = DateTime.Now.Date.AddDays(-30);

    [ObservableProperty]
    private DateTime hasta = DateTime.Now.Date;

    public ReportesViewModel(ReportesService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task GenerarVentasComprasAsync()
    {
        VentasPorProducto.Clear();
        foreach (var item in await _service.VentasPorProductoAsync(Desde, Hasta))
        {
            VentasPorProducto.Add(item);
        }

        VentasPorUsuario.Clear();
        foreach (var item in await _service.VentasPorUsuarioAsync(Desde, Hasta))
        {
            VentasPorUsuario.Add(item);
        }

        ComprasPorProveedor.Clear();
        foreach (var item in await _service.ComprasPorProveedorAsync(Desde, Hasta))
        {
            ComprasPorProveedor.Add(item);
        }

        GananciasPorDia.Clear();
        foreach (var item in await _service.GananciasPorDiaAsync(Desde, Hasta))
        {
            GananciasPorDia.Add(item);
        }
    }

    [RelayCommand]
    private async Task CargarInventarioAsync()
    {
        StockActual.Clear();
        foreach (var producto in await _service.StockActualAsync())
        {
            StockActual.Add(producto);
        }

        StockBajo.Clear();
        foreach (var producto in await _service.StockBajoAsync())
        {
            StockBajo.Add(producto);
        }

        Agotados.Clear();
        foreach (var producto in await _service.AgotadosAsync())
        {
            Agotados.Add(producto);
        }
    }
}
