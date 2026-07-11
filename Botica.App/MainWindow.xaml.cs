using System.Windows;
using Botica.App.ViewModels;
using Botica.App.Views;
using Botica.Core.Entities;
using Botica.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Botica.App;

public partial class MainWindow : Window
{
    private readonly Usuario _usuario;
    private readonly AuthService _authService;
    private readonly IServiceProvider _serviceProvider;
    private bool _cerrandoSesion;

    public event EventHandler? SesionCerrada;
    public event EventHandler? AppCerrandose;

    public MainWindow(Usuario usuario, AuthService authService, IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _usuario = usuario;
        _authService = authService;
        _serviceProvider = serviceProvider;
        UsuarioConectadoTextBlock.Text = $"{_usuario.NombreCompleto} ({_usuario.Rol})";

        Closed += MainWindow_Closed;
    }

    private async void CerrarSesionButton_Click(object sender, RoutedEventArgs e)
    {
        await _authService.CerrarSesionAsync(_usuario.Id);
        _cerrandoSesion = true;
        Close();
    }

    private void VentasButton_Click(object sender, RoutedEventArgs e)
    {
        var ventana = _serviceProvider.GetRequiredService<VentasWindow>();
        ventana.ViewModel.EstablecerUsuario(_usuario.Id);
        ventana.Owner = this;
        ventana.ShowDialog();
    }

    private void CajaButton_Click(object sender, RoutedEventArgs e)
    {
        var ventana = _serviceProvider.GetRequiredService<CajaWindow>();
        ventana.ViewModel.EstablecerUsuario(_usuario.Id);
        ventana.Owner = this;
        ventana.ShowDialog();
    }

    private void ComprasButton_Click(object sender, RoutedEventArgs e)
    {
        var ventana = _serviceProvider.GetRequiredService<ComprasWindow>();
        ventana.ViewModel.EstablecerUsuario(_usuario.Id);
        ventana.Owner = this;
        ventana.ShowDialog();
    }

    private void ProveedoresButton_Click(object sender, RoutedEventArgs e)
    {
        var ventana = _serviceProvider.GetRequiredService<ProveedoresWindow>();
        ventana.Owner = this;
        ventana.ShowDialog();
    }

    private void ProductosButton_Click(object sender, RoutedEventArgs e)
    {
        var ventana = _serviceProvider.GetRequiredService<ProductosWindow>();
        ventana.Owner = this;
        ventana.ShowDialog();
    }

    private void CategoriasButton_Click(object sender, RoutedEventArgs e)
    {
        var ventana = _serviceProvider.GetRequiredService<CategoriasWindow>();
        ventana.Owner = this;
        ventana.ShowDialog();
    }

    private void LaboratoriosButton_Click(object sender, RoutedEventArgs e)
    {
        var ventana = _serviceProvider.GetRequiredService<LaboratoriosWindow>();
        ventana.Owner = this;
        ventana.ShowDialog();
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
