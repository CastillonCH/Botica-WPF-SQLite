namespace Botica.Core.Entities;

public class Proveedor
{
    public int Id { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string? Ruc { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
