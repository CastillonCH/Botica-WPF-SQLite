namespace Botica.Core.Entities;

public class Compra
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    public Proveedor Proveedor { get; set; } = null!;
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string? NumeroComprobante { get; set; }
    public DateTime FechaCompra { get; set; } = DateTime.Now;
    public decimal Total { get; set; }

    public ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
}
