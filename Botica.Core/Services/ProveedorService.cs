using Botica.Core.Data;
using Botica.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class ProveedorService
{
    private readonly BoticaDbContext _dbContext;

    public ProveedorService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Proveedor>> ObtenerTodosAsync(string? textoBusqueda = null)
    {
        var query = _dbContext.Proveedores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(textoBusqueda))
        {
            var texto = textoBusqueda.Trim();
            query = query.Where(p => p.RazonSocial.Contains(texto) || (p.Ruc != null && p.Ruc.Contains(texto)));
        }

        return await query.OrderBy(p => p.RazonSocial).ToListAsync();
    }

    public async Task GuardarAsync(Proveedor proveedor)
    {
        if (proveedor.Id == 0)
        {
            _dbContext.Proveedores.Add(proveedor);
        }
        else
        {
            _dbContext.Proveedores.Update(proveedor);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var proveedor = await _dbContext.Proveedores.FindAsync(id);
        if (proveedor is null)
        {
            return false;
        }

        _dbContext.Proveedores.Remove(proveedor);

        try
        {
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            _dbContext.Entry(proveedor).State = EntityState.Unchanged;
            return false;
        }
    }

    public async Task<List<Compra>> ObtenerHistorialComprasAsync(int proveedorId)
    {
        return await _dbContext.Compras
            .Where(c => c.ProveedorId == proveedorId)
            .OrderByDescending(c => c.FechaCompra)
            .ToListAsync();
    }
}
