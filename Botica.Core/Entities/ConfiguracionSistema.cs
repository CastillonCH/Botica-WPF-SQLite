namespace Botica.Core.Entities;

public class ConfiguracionSistema
{
    public int Id { get; set; }
    public string NombreEmpresa { get; set; } = string.Empty;
    public string? Ruc { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public decimal TasaIgv { get; set; } = 0.18m;
    public string Moneda { get; set; } = "S/";

    public string? SmtpHost { get; set; }
    public int SmtpPuerto { get; set; } = 587;
    public string? SmtpUsuario { get; set; }
    public string? SmtpContrasena { get; set; }
    public bool SmtpUsarSsl { get; set; } = true;
    public string? SmtpRemitente { get; set; }
}
