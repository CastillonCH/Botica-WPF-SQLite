using Botica.Core.Data;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class AuthService
{
    private readonly BoticaDbContext _dbContext;

    public AuthService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Usuario?> IniciarSesionAsync(string nombreUsuario, string password)
    {
        var usuario = await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

        if (usuario is null || !PasswordHasher.Verify(password, usuario.PasswordHash))
        {
            return null;
        }

        _dbContext.HistorialAccesos.Add(new HistorialAcceso
        {
            UsuarioId = usuario.Id,
            TipoEvento = TipoEventoAcceso.Login,
            EquipoOrigen = Environment.MachineName
        });
        await _dbContext.SaveChangesAsync();

        return usuario;
    }

    public async Task CerrarSesionAsync(int usuarioId)
    {
        _dbContext.HistorialAccesos.Add(new HistorialAcceso
        {
            UsuarioId = usuarioId,
            TipoEvento = TipoEventoAcceso.Logout,
            EquipoOrigen = Environment.MachineName
        });
        await _dbContext.SaveChangesAsync();
    }
}
