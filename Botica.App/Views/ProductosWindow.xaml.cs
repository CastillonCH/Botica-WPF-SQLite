using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class ProductosWindow : Window
{
    public ProductosWindow(ProductosViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }
}
