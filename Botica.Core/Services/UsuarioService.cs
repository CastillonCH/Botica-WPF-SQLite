using Botica.Core.Data;
using Botica.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class UsuarioService
{
    private readonly BoticaDbContext _dbContext;

    public UsuarioService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _dbContext.Usuarios.OrderBy(u => u.NombreCompleto).ToListAsync();
    }

    public async Task<string?> GuardarAsync(Usuario usuario, string? nuevaContrasena)
    {
        if (usuario.Id == 0)
        {
            if (string.IsNullOrWhiteSpace(nuevaContrasena))
            {
                return "La contraseña es obligatoria para un usuario nuevo.";
            }

            usuario.PasswordHash = PasswordHasher.Hash(nuevaContrasena);
            _dbContext.Usuarios.Add(usuario);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(nuevaContrasena))
            {
                usuario.PasswordHash = PasswordHasher.Hash(nuevaContrasena);
            }

            _dbContext.Usuarios.Update(usuario);
        }

        try
        {
            await _dbContext.SaveChangesAsync();
            return null;
        }
        catch (DbUpdateException)
        {
            return "Ya existe un usuario con ese nombre de usuario.";
        }
    }

    public async Task CambiarEstadoAsync(int id)
    {
        var usuario = await _dbContext.Usuarios.FindAsync(id);
        if (usuario is null)
        {
            return;
        }

        usuario.Activo = !usuario.Activo;
        await _dbContext.SaveChangesAsync();
    }
}
