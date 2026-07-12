using Botica.Core.Enums;

namespace Botica.Core.Entities;

public class AjusteInventario
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public TipoAjusteInventario Tipo { get; set; }
    public int Cantidad { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public DateTime FechaHora { get; set; } = DateTime.Now;
}
