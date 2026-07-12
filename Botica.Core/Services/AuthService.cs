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

    public async Task<string?> CambiarContrasenaAsync(int usuarioId, string contrasenaActual, string contrasenaNueva)
    {
        var usuario = await _dbContext.Usuarios.FindAsync(usuarioId);
        if (usuario is null || !PasswordHasher.Verify(contrasenaActual, usuario.PasswordHash))
        {
            return "La contraseña actual es incorrecta.";
        }

        if (string.IsNullOrWhiteSpace(contrasenaNueva) || contrasenaNueva.Length < 4)
        {
            return "La nueva contraseña debe tener al menos 4 caracteres.";
        }

        usuario.PasswordHash = PasswordHasher.Hash(contrasenaNueva);
        await _dbContext.SaveChangesAsync();

        return null;
    }
}
