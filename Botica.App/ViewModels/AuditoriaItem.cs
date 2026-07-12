namespace Botica.App.ViewModels;

public class AuditoriaItem
{
    public DateTime FechaHora { get; init; }
    public string Usuario { get; init; } = string.Empty;
    public string Accion { get; init; } = string.Empty;
    public string Detalle { get; init; } = string.Empty;
}
