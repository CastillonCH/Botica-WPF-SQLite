using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class LaboratoriosWindow : Window
{
    public LaboratoriosWindow(LaboratoriosViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }
}
