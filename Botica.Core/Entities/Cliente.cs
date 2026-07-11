using Botica.Core.Enums;

namespace Botica.Core.Entities;

public class Cliente
{
    public int Id { get; set; }
    public TipoDocumento TipoDocumento { get; set; } = TipoDocumento.SinDocumento;
    public string? NumeroDocumento { get; set; }
    public string? NombreORazonSocial { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
