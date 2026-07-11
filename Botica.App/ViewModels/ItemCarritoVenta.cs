using CommunityToolkit.Mvvm.ComponentModel;

namespace Botica.App.ViewModels;

public partial class ItemCarritoVenta : ObservableObject
{
    public int ProductoId { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public decimal PrecioUnitario { get; init; }
    public int StockDisponible { get; init; }

    [ObservableProperty]
    private int cantidad = 1;

    [ObservableProperty]
    private decimal descuento;

    public decimal Subtotal => (PrecioUnitario * Cantidad) - Descuento;

    partial void OnCantidadChanged(int value) => OnPropertyChanged(nameof(Subtotal));

    partial void OnDescuentoChanged(decimal value) => OnPropertyChanged(nameof(Subtotal));
}
