using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class AuditoriaWindow : Window
{
    public AuditoriaWindow(AuditoriaViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }
}
