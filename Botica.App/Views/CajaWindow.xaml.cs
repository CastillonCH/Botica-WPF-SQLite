using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class CajaWindow : Window
{
    public CajaViewModel ViewModel { get; }

    public CajaWindow(CajaViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }
}
