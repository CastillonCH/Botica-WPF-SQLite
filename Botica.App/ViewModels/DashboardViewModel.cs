using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DashboardService _service;

    public ObservableCollection<ProductoMasVendido> ProductosMasVendidos { get; } = new();
    public ObservableCollection<Venta> UltimasVentas { get; } = new();
    public ObservableCollection<Compra> UltimasCompras { get; } = new();

    [ObservableProperty]
    private decimal ventasDelDia;

    [ObservableProperty]
    private decimal ventasDelMes;

    [ObservableProperty]
    private decimal comprasDelMes;

    [ObservableProperty]
    private decimal gananciaDelMes;

    [ObservableProperty]
    private int productosStockBajo;

    [ObservableProperty]
    private int productosAgotados;

    [ObservableProperty]
    private int productosProximosAVencer;

    [ObservableProperty]
    private int productosVencidos;

    public DashboardViewModel(DashboardService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        var resumen = await _service.ObtenerResumenAsync();

        VentasDelDia = resumen.VentasDelDia;
        VentasDelMes = resumen.VentasDelMes;
        ComprasDelMes = resumen.ComprasDelMes;
        GananciaDelMes = resumen.GananciaDelMes;
        ProductosStockBajo = resumen.ProductosStockBajo;
        ProductosAgotados = resumen.ProductosAgotados;
        ProductosProximosAVencer = resumen.ProductosProximosAVencer;
        ProductosVencidos = resumen.ProductosVencidos;

        ProductosMasVendidos.Clear();
        foreach (var producto in resumen.ProductosMasVendidos)
        {
            ProductosMasVendidos.Add(producto);
        }

        UltimasVentas.Clear();
        foreach (var venta in resumen.UltimasVentas)
        {
            UltimasVentas.Add(venta);
        }

        UltimasCompras.Clear();
        foreach (var compra in resumen.UltimasCompras)
        {
            UltimasCompras.Add(compra);
        }
    }
}
