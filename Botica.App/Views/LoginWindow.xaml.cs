using System.Windows;
using System.Windows.Input;
using Botica.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Botica.App.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;
    private readonly IServiceProvider _serviceProvider;

    public LoginWindow(LoginViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
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

    private void OlvideContrasenaButton_Click(object sender, RoutedEventArgs e)
    {
        var ventana = _serviceProvider.GetRequiredService<RecuperarContrasenaWindow>();
        ventana.Owner = this;
        ventana.ShowDialog();
    }
}
