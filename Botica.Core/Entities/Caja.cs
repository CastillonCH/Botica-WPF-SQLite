using Botica.Core.Enums;

namespace Botica.Core.Entities;

public class Caja
{
    public int Id { get; set; }
    public int UsuarioAperturaId { get; set; }
    public Usuario UsuarioApertura { get; set; } = null!;
    public int? UsuarioCierreId { get; set; }
    public Usuario? UsuarioCierre { get; set; }
    public DateTime FechaApertura { get; set; } = DateTime.Now;
    public decimal MontoApertura { get; set; }
    public DateTime? FechaCierre { get; set; }
    public decimal? MontoCierreDeclarado { get; set; }
    public decimal? MontoCierreSistema { get; set; }
    public EstadoCaja Estado { get; set; } = EstadoCaja.Abierta;
    public string? Observaciones { get; set; }

    public ICollection<MovimientoCaja> Movimientos { get; set; } = new List<MovimientoCaja>();
    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
