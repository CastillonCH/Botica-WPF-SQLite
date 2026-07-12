using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class CategoriasWindow : Window
{
    public CategoriasWindow(CategoriasViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }
}
