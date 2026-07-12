using Botica.Core.Data;
using Botica.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class DashboardService
{
    private readonly BoticaDbContext _dbContext;

    public DashboardService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardResumen> ObtenerResumenAsync()
    {
        var hoy = DateTime.Now.Date;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        var limiteVencimiento = hoy.AddDays(90);

        var ventasEmitidas = _dbContext.Ventas.Where(v => v.Estado == EstadoVenta.Emitida);

        // SQLite no soporta SUM sobre columnas decimal a nivel de SQL: se trae la
        // lista y se suma en memoria.
        var ventasDelDia = (await ventasEmitidas
            .Where(v => v.FechaHora.Date == hoy)
            .Select(v => v.Total)
            .ToListAsync()).Sum();

        var ventasDelMes = (await ventasEmitidas
            .Where(v => v.FechaHora >= inicioMes)
            .Select(v => v.Total)
            .ToListAsync()).Sum();

        var comprasDelMes = (await _dbContext.Compras
            .Where(c => c.FechaCompra >= inicioMes)
            .Select(c => c.Total)
            .ToListAsync()).Sum();

        var detallesDelMes = await _dbContext.DetallesVenta
            .Include(d => d.Producto)
            .Where(d => d.Venta.Estado == EstadoVenta.Emitida && d.Venta.FechaHora >= inicioMes)
            .ToListAsync();

        var gananciaDelMes = detallesDelMes.Sum(d => d.Subtotal - (d.Cantidad * d.Producto.PrecioCompra));

        var productosStockBajo = await _dbContext.Productos
            .CountAsync(p => p.Activo && p.StockActual > 0 && p.StockActual <= p.StockMinimo);

        var productosAgotados = await _dbContext.Productos
            .CountAsync(p => p.Activo && p.StockActual == 0);

        var productosProximosAVencer = await _dbContext.Lotes
            .CountAsync(l => l.CantidadActual > 0 && l.FechaVencimiento >= hoy && l.FechaVencimiento <= limiteVencimiento);

        var productosVencidos = await _dbContext.Lotes
            .CountAsync(l => l.CantidadActual > 0 && l.FechaVencimiento < hoy);

        var productosMasVendidos = detallesDelMes
            .GroupBy(d => d.Producto.Nombre)
            .Select(g => new ProductoMasVendido { Nombre = g.Key, CantidadVendida = g.Sum(d => d.Cantidad) })
            .OrderByDescending(p => p.CantidadVendida)
            .Take(5)
            .ToList();

        var ultimasVentas = await ventasEmitidas
            .OrderByDescending(v => v.FechaHora)
            .Take(5)
            .ToListAsync();

        var ultimasCompras = await _dbContext.Compras
            .Include(c => c.Proveedor)
            .OrderByDescending(c => c.FechaCompra)
            .Take(5)
            .ToListAsync();

        return new DashboardResumen
        {
            VentasDelDia = ventasDelDia,
            VentasDelMes = ventasDelMes,
            ComprasDelMes = comprasDelMes,
            GananciaDelMes = gananciaDelMes,
            ProductosStockBajo = productosStockBajo,
            ProductosAgotados = productosAgotados,
            ProductosProximosAVencer = productosProximosAVencer,
            ProductosVencidos = productosVencidos,
            ProductosMasVendidos = productosMasVendidos,
            UltimasVentas = ultimasVentas,
            UltimasCompras = ultimasCompras
        };
    }
}
