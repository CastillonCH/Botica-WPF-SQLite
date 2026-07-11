using Botica.Core.Data;
using Botica.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class CompraService
{
    private readonly BoticaDbContext _dbContext;

    public CompraService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Compra> RegistrarCompraAsync(Compra compra)
    {
        foreach (var detalle in compra.Detalles)
        {
            var producto = await _dbContext.Productos.FindAsync(detalle.ProductoId)
                ?? throw new InvalidOperationException("Producto no encontrado.");

            producto.StockActual += detalle.Cantidad;
            producto.PrecioCompra = detalle.PrecioUnitario;

            var lote = await _dbContext.Lotes.FirstOrDefaultAsync(l =>
                l.ProductoId == detalle.ProductoId && l.NumeroLote == detalle.NumeroLote);

            if (lote is not null)
            {
                lote.CantidadInicial += detalle.Cantidad;
                lote.CantidadActual += detalle.Cantidad;
                lote.FechaVencimiento = detalle.FechaVencimiento;
            }
            else
            {
                _dbContext.Lotes.Add(new Lote
                {
                    ProductoId = detalle.ProductoId,
                    NumeroLote = detalle.NumeroLote,
                    FechaVencimiento = detalle.FechaVencimiento,
                    CantidadInicial = detalle.Cantidad,
                    CantidadActual = detalle.Cantidad
                });
            }
        }

        _dbContext.Compras.Add(compra);
        await _dbContext.SaveChangesAsync();

        return compra;
    }

    public async Task<List<Compra>> ObtenerTodosAsync()
    {
        return await _dbContext.Compras
            .Include(c => c.Proveedor)
            .Include(c => c.Detalles)
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }
}
