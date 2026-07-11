using Botica.Core.Data;
using Botica.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class LoteService
{
    private readonly BoticaDbContext _dbContext;

    public LoteService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Lote>> ObtenerPorProductoAsync(int productoId)
    {
        return await _dbContext.Lotes
            .Where(l => l.ProductoId == productoId)
            .OrderBy(l => l.FechaVencimiento)
            .ToListAsync();
    }

    public async Task<List<Lote>> ObtenerProximosAVencerAsync(int dias)
    {
        var limite = DateTime.Now.Date.AddDays(dias);

        return await _dbContext.Lotes
            .Include(l => l.Producto)
            .Where(l => l.CantidadActual > 0 && l.FechaVencimiento <= limite && l.FechaVencimiento >= DateTime.Now.Date)
            .OrderBy(l => l.FechaVencimiento)
            .ToListAsync();
    }

    public async Task<List<Lote>> ObtenerVencidosAsync()
    {
        return await _dbContext.Lotes
            .Include(l => l.Producto)
            .Where(l => l.CantidadActual > 0 && l.FechaVencimiento < DateTime.Now.Date)
            .OrderBy(l => l.FechaVencimiento)
            .ToListAsync();
    }
}
