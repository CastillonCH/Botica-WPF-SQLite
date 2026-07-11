using CommunityToolkit.Mvvm.ComponentModel;

namespace Botica.App.ViewModels;

public partial class ItemCarritoCompra : ObservableObject
{
    public int ProductoId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;

    [ObservableProperty]
    private int cantidad = 1;

    [ObservableProperty]
    private decimal precioUnitario;

    [ObservableProperty]
    private string numeroLote = string.Empty;

    [ObservableProperty]
    private DateTime fechaVencimiento = DateTime.Now.AddYears(1);

    public decimal Subtotal => PrecioUnitario * Cantidad;

    partial void OnCantidadChanged(int value) => OnPropertyChanged(nameof(Subtotal));

    partial void OnPrecioUnitarioChanged(decimal value) => OnPropertyChanged(nameof(Subtotal));
}
