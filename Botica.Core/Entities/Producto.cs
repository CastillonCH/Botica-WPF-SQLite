namespace Botica.Core.Entities;

public class Producto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string? CodigoBarras { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? NombreGenerico { get; set; }
    public int? LaboratorioId { get; set; }
    public Laboratorio? Laboratorio { get; set; }
    public int? CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public string? Presentacion { get; set; }
    public string? Concentracion { get; set; }
    public string? UnidadMedida { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockMinimo { get; set; }
    public int StockActual { get; set; }
    public bool RequiereReceta { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
    public ICollection<Lote> Lotes { get; set; } = new List<Lote>();
}
