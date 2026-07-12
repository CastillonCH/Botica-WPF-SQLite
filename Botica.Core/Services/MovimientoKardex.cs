namespace Botica.Core.Services;

public class MovimientoKardex
{
    public DateTime FechaHora { get; set; }
    public string TipoMovimiento { get; set; } = string.Empty;
    public int Entrada { get; set; }
    public int Salida { get; set; }
    public int Saldo { get; set; }
    public string Detalle { get; set; } = string.Empty;
}
