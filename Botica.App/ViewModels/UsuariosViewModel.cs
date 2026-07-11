using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class UsuariosViewModel : ObservableObject
{
    private readonly UsuarioService _service;

    public ObservableCollection<Usuario> Usuarios { get; } = new();
    public IEnumerable<RolUsuario> Roles => Enum.GetValues<RolUsuario>();

    [ObservableProperty]
    private Usuario usuarioSeleccionado = new() { Activo = true };

    [ObservableProperty]
    private string mensaje = string.Empty;

    public UsuariosViewModel(UsuarioService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        Usuarios.Clear();
        foreach (var usuario in await _service.ObtenerTodosAsync())
        {
            Usuarios.Add(usuario);
        }
    }

    [RelayCommand]
    private void Nuevo()
    {
        UsuarioSeleccionado = new Usuario { Activo = true };
        Mensaje = string.Empty;
    }

    [RelayCommand]
    private async Task GuardarAsync(string? nuevaContrasena)
    {
        if (string.IsNullOrWhiteSpace(UsuarioSeleccionado.NombreUsuario) ||
            string.IsNullOrWhiteSpace(UsuarioSeleccionado.NombreCompleto))
        {
            Mensaje = "Usuario y nombre completo son obligatorios.";
            return;
        }

        var error = await _service.GuardarAsync(UsuarioSeleccionado, nuevaContrasena);
        if (error is not null)
        {
            Mensaje = error;
            return;
        }

        Mensaje = string.Empty;
        await CargarAsync();
        Nuevo();
    }

    [RelayCommand]
    private async Task CambiarEstadoAsync()
    {
        if (UsuarioSeleccionado.Id == 0)
        {
            return;
        }

        await _service.CambiarEstadoAsync(UsuarioSeleccionado.Id);
        await CargarAsync();
        Nuevo();
    }
}
