using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class ProveedoresWindow : Window
{
    public ProveedoresWindow(ProveedoresViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }
}
