using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class ReportesWindow : Window
{
    public ReportesViewModel ViewModel { get; }

    public ReportesWindow(ReportesViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (_, _) =>
        {
            await viewModel.GenerarVentasComprasCommand.ExecuteAsync(null);
            await viewModel.CargarInventarioCommand.ExecuteAsync(null);
        };
    }
}
