using Botica.Core.Entities;

namespace Botica.Core.Services;

public class DashboardResumen
{
    public decimal VentasDelDia { get; set; }
    public decimal VentasDelMes { get; set; }
    public decimal ComprasDelMes { get; set; }
    public decimal GananciaDelMes { get; set; }
    public int ProductosStockBajo { get; set; }
    public int ProductosAgotados { get; set; }
    public int ProductosProximosAVencer { get; set; }
    public int ProductosVencidos { get; set; }
    public List<ProductoMasVendido> ProductosMasVendidos { get; set; } = new();
    public List<Venta> UltimasVentas { get; set; } = new();
    public List<Compra> UltimasCompras { get; set; } = new();
}

public class ProductoMasVendido
{
    public string Nombre { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
}
