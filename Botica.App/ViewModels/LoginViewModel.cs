using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [ObservableProperty]
    private string nombreUsuario = string.Empty;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    [ObservableProperty]
    private bool iniciandoSesion;

    public Usuario? UsuarioAutenticado { get; private set; }

    public event EventHandler? InicioSesionExitoso;

    [RelayCommand]
    private async Task IniciarSesionAsync(string password)
    {
        MensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrWhiteSpace(password))
        {
            MensajeError = "Ingrese usuario y contraseña.";
            return;
        }

        IniciandoSesion = true;
        try
        {
            var usuario = await _authService.IniciarSesionAsync(NombreUsuario.Trim(), password);
            if (usuario is null)
            {
                MensajeError = "Usuario o contraseña incorrectos.";
                return;
            }

            UsuarioAutenticado = usuario;
            InicioSesionExitoso?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            IniciandoSesion = false;
        }
    }
}
