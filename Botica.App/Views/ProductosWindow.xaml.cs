using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class ProductosWindow : Window
{
    public ProductosViewModel ViewModel { get; }

    public ProductosWindow(ProductosViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }
}
