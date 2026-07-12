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
}
