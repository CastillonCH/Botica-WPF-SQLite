using Botica.Core.Data;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class AuditoriaService
{
    private readonly BoticaDbContext _dbContext;

    public AuditoriaService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task RegistrarAsync(int usuarioId, string tipoEntidad, int entidadId, AccionAuditoria accion,
        string detalle)
    {
        _dbContext.AuditoriaEventos.Add(new AuditoriaEvento
        {
            UsuarioId = usuarioId,
            TipoEntidad = tipoEntidad,
            EntidadId = entidadId,
            Accion = accion,
            Detalle = detalle
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<AuditoriaEvento>> ObtenerTodosAsync(int limite = 200)
    {
        return await _dbContext.AuditoriaEventos
            .Include(a => a.Usuario)
            .OrderByDescending(a => a.FechaHora)
            .Take(limite)
            .ToListAsync();
    }
}
