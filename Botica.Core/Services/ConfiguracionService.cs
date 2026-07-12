using Botica.Core.Data;
using Botica.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class ConfiguracionService
{
    private readonly BoticaDbContext _dbContext;

    public ConfiguracionService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ConfiguracionSistema> ObtenerAsync()
    {
        var configuracion = await _dbContext.ConfiguracionSistema.FirstOrDefaultAsync();
        if (configuracion is not null)
        {
            return configuracion;
        }

        configuracion = new ConfiguracionSistema { NombreEmpresa = "Mi Botica" };
        _dbContext.ConfiguracionSistema.Add(configuracion);
        await _dbContext.SaveChangesAsync();

        return configuracion;
    }

    public async Task GuardarAsync(ConfiguracionSistema configuracion)
    {
        _dbContext.ConfiguracionSistema.Update(configuracion);
        await _dbContext.SaveChangesAsync();
    }
}
