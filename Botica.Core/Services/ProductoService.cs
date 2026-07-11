using Botica.Core.Data;
using Botica.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class ProductoService
{
    private readonly BoticaDbContext _dbContext;

    public ProductoService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Producto>> ObtenerTodosAsync(string? textoBusqueda = null)
    {
        var query = _dbContext.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Laboratorio)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(textoBusqueda))
        {
            var texto = textoBusqueda.Trim();
            query = query.Where(p =>
                p.Nombre.Contains(texto) ||
                p.Codigo.Contains(texto) ||
                (p.CodigoBarras != null && p.CodigoBarras.Contains(texto)));
        }

        return await query.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task GuardarAsync(Producto producto)
    {
        if (producto.Id == 0)
        {
            _dbContext.Productos.Add(producto);
        }
        else
        {
            _dbContext.Productos.Update(producto);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task CambiarEstadoAsync(int id)
    {
        var producto = await _dbContext.Productos.FindAsync(id);
        if (producto is null)
        {
            return;
        }

        producto.Activo = !producto.Activo;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var producto = await _dbContext.Productos.FindAsync(id);
        if (producto is null)
        {
            return false;
        }

        _dbContext.Productos.Remove(producto);

        try
        {
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            _dbContext.Entry(producto).State = EntityState.Unchanged;
            return false;
        }
    }
}
