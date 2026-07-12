using Botica.Core.Enums;

namespace Botica.Core.Entities;

public class Venta
{
    public int Id { get; set; }
    public string? Serie { get; set; }
    public int? Numero { get; set; }
    public TipoComprobante TipoComprobante { get; set; } = TipoComprobante.Ticket;
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int CajaId { get; set; }
    public Caja Caja { get; set; } = null!;
    public DateTime FechaHora { get; set; } = DateTime.Now;
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public MetodoPago MetodoPago { get; set; }
    public decimal? MontoRecibido { get; set; }
    public decimal? Vuelto { get; set; }
    public EstadoVenta Estado { get; set; } = EstadoVenta.Emitida;
    public string? MotivoAnulacion { get; set; }

    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}
