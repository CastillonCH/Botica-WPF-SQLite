using Botica.Core.Data;
using Botica.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class LaboratorioService
{
    private readonly BoticaDbContext _dbContext;

    public LaboratorioService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Laboratorio>> ObtenerTodosAsync()
    {
        return await _dbContext.Laboratorios.OrderBy(l => l.Nombre).ToListAsync();
    }

    public async Task GuardarAsync(Laboratorio laboratorio)
    {
        if (laboratorio.Id == 0)
        {
            _dbContext.Laboratorios.Add(laboratorio);
        }
        else
        {
            _dbContext.Laboratorios.Update(laboratorio);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var laboratorio = await _dbContext.Laboratorios.FindAsync(id);
        if (laboratorio is null)
        {
            return false;
        }

        _dbContext.Laboratorios.Remove(laboratorio);

        try
        {
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            _dbContext.Entry(laboratorio).State = EntityState.Unchanged;
            return false;
        }
    }
}
