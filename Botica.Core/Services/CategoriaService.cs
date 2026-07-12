using Botica.Core.Data;
using Botica.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class CategoriaService
{
    private readonly BoticaDbContext _dbContext;

    public CategoriaService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Categoria>> ObtenerTodosAsync()
    {
        return await _dbContext.Categorias.OrderBy(c => c.Nombre).ToListAsync();
    }

    public async Task GuardarAsync(Categoria categoria)
    {
        if (categoria.Id == 0)
        {
            _dbContext.Categorias.Add(categoria);
        }
        else
        {
            _dbContext.Categorias.Update(categoria);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var categoria = await _dbContext.Categorias.FindAsync(id);
        if (categoria is null)
        {
            return false;
        }

        _dbContext.Categorias.Remove(categoria);

        try
        {
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            _dbContext.Entry(categoria).State = EntityState.Unchanged;
            return false;
        }
    }
}
