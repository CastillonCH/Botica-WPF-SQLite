using System.Windows;
using Botica.Core.Entities;
using Botica.Core.Services;

namespace Botica.App;

public partial class MainWindow : Window
{
    private readonly Usuario _usuario;
    private readonly AuthService _authService;
    private bool _cerrandoSesion;

    public event EventHandler? SesionCerrada;
    public event EventHandler? AppCerrandose;

    public MainWindow(Usuario usuario, AuthService authService)
    {
        InitializeComponent();

        _usuario = usuario;
        _authService = authService;
        UsuarioConectadoTextBlock.Text = $"{_usuario.NombreCompleto} ({_usuario.Rol})";

        Closed += MainWindow_Closed;
    }

    private async void CerrarSesionButton_Click(object sender, RoutedEventArgs e)
    {
        await _authService.CerrarSesionAsync(_usuario.Id);
        _cerrandoSesion = true;
        Close();
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        if (_cerrandoSesion)
        {
            SesionCerrada?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            AppCerrandose?.Invoke(this, EventArgs.Empty);
        }
    }
}
