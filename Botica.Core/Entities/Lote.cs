namespace Botica.Core.Entities;

public class Lote
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;
    public string NumeroLote { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; } = DateTime.Now;
    public DateTime FechaVencimiento { get; set; }
    public int CantidadInicial { get; set; }
    public int CantidadActual { get; set; }
}
