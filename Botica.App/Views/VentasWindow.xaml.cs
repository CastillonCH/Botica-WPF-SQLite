using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class VentasWindow : Window
{
    public VentasViewModel ViewModel { get; }

    public VentasWindow(VentasViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }
}
