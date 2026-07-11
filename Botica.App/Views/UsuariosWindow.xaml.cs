using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class UsuariosWindow : Window
{
    private readonly UsuariosViewModel _viewModel;

    public UsuariosWindow(UsuariosViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.CargarCommand.ExecuteAsync(null);
    }

    private async void GuardarButton_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.GuardarCommand.ExecuteAsync(PasswordBox.Password);
        PasswordBox.Clear();
    }
}
