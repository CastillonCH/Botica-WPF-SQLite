using Botica.Core.Enums;

namespace Botica.Core.Entities;

public class MovimientoCaja
{
    public int Id { get; set; }
    public int CajaId { get; set; }
    public Caja Caja { get; set; } = null!;
    public TipoMovimientoCaja Tipo { get; set; }
    public string Concepto { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.Now;
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
