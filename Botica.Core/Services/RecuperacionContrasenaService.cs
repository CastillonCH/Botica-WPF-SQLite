using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Botica.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class RecuperacionContrasenaService
{
    private static readonly TimeSpan VigenciaCodigo = TimeSpan.FromMinutes(15);

    private readonly BoticaDbContext _dbContext;
    private readonly ConfiguracionService _configuracionService;

    public RecuperacionContrasenaService(BoticaDbContext dbContext, ConfiguracionService configuracionService)
    {
        _dbContext = dbContext;
        _configuracionService = configuracionService;
    }

    public async Task<string?> SolicitarCodigoAsync(string nombreUsuario)
    {
        var usuario = await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

        if (usuario is null)
        {
            return "No existe un usuario activo con ese nombre de usuario.";
        }

        if (string.IsNullOrWhiteSpace(usuario.Email))
        {
            return "Este usuario no tiene un correo registrado. Pide a un administrador que te restablezca la contraseña desde el módulo Usuarios.";
        }

        var configuracion = await _configuracionService.ObtenerAsync();
        if (string.IsNullOrWhiteSpace(configuracion.SmtpHost) || string.IsNullOrWhiteSpace(configuracion.SmtpUsuario))
        {
            return "El envío de correo no está configurado. Pide a un administrador que configure el SMTP en Configuración, o que te restablezca la contraseña desde el módulo Usuarios.";
        }

        var codigo = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        usuario.CodigoRecuperacion = codigo;
        usuario.CodigoRecuperacionExpira = DateTime.Now.Add(VigenciaCodigo);
        await _dbContext.SaveChangesAsync();

        try
        {
            await EnviarCorreoAsync(configuracion, usuario.Email, codigo);
        }
        catch (Exception ex)
        {
            return $"No se pudo enviar el correo: {ex.Message}";
        }

        return null;
    }

    public async Task<string?> RestablecerConCodigoAsync(string nombreUsuario, string codigo, string nuevaContrasena)
    {
        var usuario = await _dbContext.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);

        if (usuario is null || string.IsNullOrWhiteSpace(usuario.CodigoRecuperacion))
        {
            return "No hay una solicitud de recuperación vigente para este usuario.";
        }

        if (usuario.CodigoRecuperacionExpira is null || usuario.CodigoRecuperacionExpira < DateTime.Now)
        {
            return "El código expiró. Solicita uno nuevo.";
        }

        if (!string.Equals(usuario.CodigoRecuperacion, codigo?.Trim(), StringComparison.Ordinal))
        {
            return "El código ingresado no es correcto.";
        }

        if (string.IsNullOrWhiteSpace(nuevaContrasena) || nuevaContrasena.Length < 4)
        {
            return "La nueva contraseña debe tener al menos 4 caracteres.";
        }

        usuario.PasswordHash = PasswordHasher.Hash(nuevaContrasena);
        usuario.CodigoRecuperacion = null;
        usuario.CodigoRecuperacionExpira = null;
        await _dbContext.SaveChangesAsync();

        return null;
    }

    private static async Task EnviarCorreoAsync(Entities.ConfiguracionSistema configuracion, string destinatario, string codigo)
    {
        using var mensaje = new MailMessage
        {
            From = new MailAddress(configuracion.SmtpUsuario!, configuracion.SmtpRemitente ?? configuracion.NombreEmpresa),
            Subject = "Código de recuperación de contraseña - Botica",
            Body = $"Tu código de recuperación es: {codigo}\n\nVence en 15 minutos. Si no solicitaste este código, ignora este mensaje.",
            IsBodyHtml = false
        };
        mensaje.To.Add(destinatario);

        using var cliente = new SmtpClient(configuracion.SmtpHost, configuracion.SmtpPuerto)
        {
            EnableSsl = configuracion.SmtpUsarSsl,
            Credentials = new NetworkCredential(configuracion.SmtpUsuario, configuracion.SmtpContrasena)
        };

        await cliente.SendMailAsync(mensaje);
    }
}
