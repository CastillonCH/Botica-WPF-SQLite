using System.Windows;
using System.Windows.Input;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
        _viewModel.InicioSesionExitoso += (_, _) =>
        {
            DialogResult = true;
            Close();
        };
    }

    private async void IngresarButton_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.IniciarSesionCommand.ExecuteAsync(PasswordBox.Password);
    }

    private async void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            await _viewModel.IniciarSesionCommand.ExecuteAsync(PasswordBox.Password);
        }
    }
}
