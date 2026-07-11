using Botica.Core.Enums;

namespace Botica.Core.Entities;

public class HistorialAcceso
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public TipoEventoAcceso TipoEvento { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.Now;
    public string? EquipoOrigen { get; set; }
}
