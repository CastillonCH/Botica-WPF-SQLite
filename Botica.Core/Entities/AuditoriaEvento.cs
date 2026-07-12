using Botica.Core.Enums;

namespace Botica.Core.Entities;

public class AuditoriaEvento
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string TipoEntidad { get; set; } = string.Empty;
    public int EntidadId { get; set; }
    public AccionAuditoria Accion { get; set; }
    public string Detalle { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; } = DateTime.Now;
}
