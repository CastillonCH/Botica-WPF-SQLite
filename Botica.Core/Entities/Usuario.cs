using Botica.Core.Enums;

namespace Botica.Core.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public ICollection<HistorialAcceso> HistorialAccesos { get; set; } = new List<HistorialAcceso>();
}
