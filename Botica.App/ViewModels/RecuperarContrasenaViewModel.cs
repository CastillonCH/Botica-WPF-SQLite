using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class RecuperarContrasenaViewModel : ObservableObject
{
    private readonly RecuperacionContrasenaService _service;

    [ObservableProperty]
    private string nombreUsuario = string.Empty;

    [ObservableProperty]
    private string codigo = string.Empty;

    [ObservableProperty]
    private string nuevaContrasena = string.Empty;

    [ObservableProperty]
    private string confirmarContrasena = string.Empty;

    [ObservableProperty]
    private string mensaje = string.Empty;

    [ObservableProperty]
    private bool codigoEnviado;

    [ObservableProperty]
    private bool procesando;

    public event EventHandler? RestablecimientoExitoso;

    public RecuperarContrasenaViewModel(RecuperacionContrasenaService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task EnviarCodigoAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreUsuario))
        {
            Mensaje = "Ingresa tu usuario.";
            return;
        }

        Procesando = true;
        try
        {
            var error = await _service.SolicitarCodigoAsync(NombreUsuario.Trim());
            if (error is not null)
            {
                Mensaje = error;
                return;
            }

            CodigoEnviado = true;
            Mensaje = "Se envió un código a tu correo. Revisa tu bandeja (y la carpeta de spam).";
        }
        finally
        {
            Procesando = false;
        }
    }

    [RelayCommand]
    private async Task RestablecerAsync()
    {
        if (NuevaContrasena != ConfirmarContrasena)
        {
            Mensaje = "La nueva contraseña y su confirmación no coinciden.";
            return;
        }

        Procesando = true;
        try
        {
            var error = await _service.RestablecerConCodigoAsync(NombreUsuario.Trim(), Codigo, NuevaContrasena);
            if (error is not null)
            {
                Mensaje = error;
                return;
            }

            Mensaje = "Contraseña actualizada. Ya puedes iniciar sesión.";
            RestablecimientoExitoso?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            Procesando = false;
        }
    }
}
