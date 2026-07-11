using Botica.Core.Data;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class VentaService
{
    private readonly BoticaDbContext _dbContext;

    public VentaService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Venta> RegistrarVentaAsync(Venta venta)
    {
        foreach (var detalle in venta.Detalles)
        {
            var producto = await _dbContext.Productos.FindAsync(detalle.ProductoId)
                ?? throw new InvalidOperationException("Producto no encontrado.");

            if (producto.StockActual < detalle.Cantidad)
            {
                throw new InvalidOperationException($"Stock insuficiente para \"{producto.Nombre}\".");
            }

            producto.StockActual -= detalle.Cantidad;
            await ConsumirLotesFefoAsync(detalle.ProductoId, detalle.Cantidad);
        }

        _dbContext.Ventas.Add(venta);
        await _dbContext.SaveChangesAsync();

        return venta;
    }

    private async Task ConsumirLotesFefoAsync(int productoId, int cantidad)
    {
        var lotes = await _dbContext.Lotes
            .Where(l => l.ProductoId == productoId && l.CantidadActual > 0)
            .OrderBy(l => l.FechaVencimiento)
            .ToListAsync();

        var cantidadRestante = cantidad;

        foreach (var lote in lotes)
        {
            if (cantidadRestante <= 0)
            {
                break;
            }

            var consumo = Math.Min(lote.CantidadActual, cantidadRestante);
            lote.CantidadActual -= consumo;
            cantidadRestante -= consumo;
        }
    }

    public async Task<bool> AnularVentaAsync(int ventaId, string motivo)
    {
        var venta = await _dbContext.Ventas
            .Include(v => v.Detalles)
            .FirstOrDefaultAsync(v => v.Id == ventaId);

        if (venta is null || venta.Estado == EstadoVenta.Anulada)
        {
            return false;
        }

        foreach (var detalle in venta.Detalles)
        {
            var producto = await _dbContext.Productos.FindAsync(detalle.ProductoId);
            if (producto is not null)
            {
                producto.StockActual += detalle.Cantidad;
            }
        }

        venta.Estado = EstadoVenta.Anulada;
        venta.MotivoAnulacion = motivo;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<Venta>> ObtenerPorCajaAsync(int cajaId)
    {
        return await _dbContext.Ventas
            .Include(v => v.Detalles)
            .Where(v => v.CajaId == cajaId)
            .OrderByDescending(v => v.FechaHora)
            .ToListAsync();
    }
}
