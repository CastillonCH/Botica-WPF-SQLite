namespace Botica.Core.Services;

public class VentaPorProductoDto
{
    public string Producto { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal Total { get; set; }
}

public class VentaPorUsuarioDto
{
    public string Usuario { get; set; } = string.Empty;
    public int CantidadVentas { get; set; }
    public decimal Total { get; set; }
}

public class CompraPorProveedorDto
{
    public string Proveedor { get; set; } = string.Empty;
    public int CantidadCompras { get; set; }
    public decimal Total { get; set; }
}

public class GananciaPorDiaDto
{
    public DateTime Fecha { get; set; }
    public decimal Ventas { get; set; }
    public decimal CostoEstimado { get; set; }
    public decimal Ganancia { get; set; }
}
