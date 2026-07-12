using Botica.Core.Data;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class ReportesService
{
    private readonly BoticaDbContext _dbContext;

    public ReportesService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Venta>> VentasDetalladoAsync(DateTime desde, DateTime hasta)
    {
        return await _dbContext.Ventas
            .Include(v => v.Usuario)
            .Where(v => v.Estado == EstadoVenta.Emitida && v.FechaHora >= desde && v.FechaHora < hasta.AddDays(1))
            .OrderByDescending(v => v.FechaHora)
            .ToListAsync();
    }

    public async Task<List<VentaPorProductoDto>> VentasPorProductoAsync(DateTime desde, DateTime hasta)
    {
        var detalles = await _dbContext.DetallesVenta
            .Include(d => d.Producto)
            .Where(d => d.Venta.Estado == EstadoVenta.Emitida && d.Venta.FechaHora >= desde &&
                        d.Venta.FechaHora < hasta.AddDays(1))
            .ToListAsync();

        return detalles
            .GroupBy(d => d.Producto.Nombre)
            .Select(g => new VentaPorProductoDto
            {
                Producto = g.Key,
                Cantidad = g.Sum(d => d.Cantidad),
                Total = g.Sum(d => d.Subtotal)
            })
            .OrderByDescending(v => v.Cantidad)
            .ToList();
    }

    public async Task<List<VentaPorUsuarioDto>> VentasPorUsuarioAsync(DateTime desde, DateTime hasta)
    {
        var ventas = await _dbContext.Ventas
            .Include(v => v.Usuario)
            .Where(v => v.Estado == EstadoVenta.Emitida && v.FechaHora >= desde && v.FechaHora < hasta.AddDays(1))
            .ToListAsync();

        return ventas
            .GroupBy(v => v.Usuario.NombreCompleto)
            .Select(g => new VentaPorUsuarioDto
            {
                Usuario = g.Key,
                CantidadVentas = g.Count(),
                Total = g.Sum(v => v.Total)
            })
            .OrderByDescending(v => v.Total)
            .ToList();
    }

    public async Task<List<Compra>> ComprasDetalladoAsync(DateTime desde, DateTime hasta)
    {
        return await _dbContext.Compras
            .Include(c => c.Proveedor)
            .Where(c => c.FechaCompra >= desde && c.FechaCompra < hasta.AddDays(1))
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }

    public async Task<List<CompraPorProveedorDto>> ComprasPorProveedorAsync(DateTime desde, DateTime hasta)
    {
        var compras = await _dbContext.Compras
            .Include(c => c.Proveedor)
            .Where(c => c.FechaCompra >= desde && c.FechaCompra < hasta.AddDays(1))
            .ToListAsync();

        return compras
            .GroupBy(c => c.Proveedor.RazonSocial)
            .Select(g => new CompraPorProveedorDto
            {
                Proveedor = g.Key,
                CantidadCompras = g.Count(),
                Total = g.Sum(c => c.Total)
            })
            .OrderByDescending(c => c.Total)
            .ToList();
    }

    public async Task<List<GananciaPorDiaDto>> GananciasPorDiaAsync(DateTime desde, DateTime hasta)
    {
        var detalles = await _dbContext.DetallesVenta
            .Include(d => d.Producto)
            .Include(d => d.Venta)
            .Where(d => d.Venta.Estado == EstadoVenta.Emitida && d.Venta.FechaHora >= desde &&
                        d.Venta.FechaHora < hasta.AddDays(1))
            .ToListAsync();

        return detalles
            .GroupBy(d => d.Venta.FechaHora.Date)
            .Select(g => new GananciaPorDiaDto
            {
                Fecha = g.Key,
                Ventas = g.Sum(d => d.Subtotal),
                CostoEstimado = g.Sum(d => d.Cantidad * d.Producto.PrecioCompra),
                Ganancia = g.Sum(d => d.Subtotal) - g.Sum(d => d.Cantidad * d.Producto.PrecioCompra)
            })
            .OrderBy(g => g.Fecha)
            .ToList();
    }

    public async Task<List<Producto>> StockActualAsync()
    {
        return await _dbContext.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Laboratorio)
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<List<Producto>> StockBajoAsync()
    {
        return await _dbContext.Productos
            .Where(p => p.Activo && p.StockActual > 0 && p.StockActual <= p.StockMinimo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<List<Producto>> AgotadosAsync()
    {
        return await _dbContext.Productos
            .Where(p => p.Activo && p.StockActual == 0)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }
}
