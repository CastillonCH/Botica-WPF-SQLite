using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class InventarioWindow : Window
{
    public InventarioViewModel ViewModel { get; }

    public InventarioWindow(InventarioViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.BuscarCommand.ExecuteAsync(null);
    }
}
