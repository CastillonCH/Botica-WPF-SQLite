using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class CambiarContrasenaViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private int _usuarioId;

    [ObservableProperty]
    private string mensaje = string.Empty;

    public event EventHandler? CambioExitoso;

    public CambiarContrasenaViewModel(AuthService authService)
    {
        _authService = authService;
    }

    public void EstablecerUsuario(int usuarioId) => _usuarioId = usuarioId;

    [RelayCommand]
    private async Task GuardarAsync(object? parametro)
    {
        var valores = (object[])parametro!;
        var actual = (string)valores[0];
        var nueva = (string)valores[1];
        var confirmacion = (string)valores[2];

        if (nueva != confirmacion)
        {
            Mensaje = "La nueva contraseña y su confirmación no coinciden.";
            return;
        }

        var error = await _authService.CambiarContrasenaAsync(_usuarioId, actual, nueva);
        if (error is not null)
        {
            Mensaje = error;
            return;
        }

        Mensaje = string.Empty;
        CambioExitoso?.Invoke(this, EventArgs.Empty);
    }
}
