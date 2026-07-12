using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class ComprasWindow : Window
{
    public ComprasViewModel ViewModel { get; }

    public ComprasWindow(ComprasViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }
}
