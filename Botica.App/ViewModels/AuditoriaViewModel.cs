using System.Collections.ObjectModel;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class AuditoriaViewModel : ObservableObject
{
    private readonly AuditoriaService _auditoriaService;
    private readonly AuthService _authService;

    public ObservableCollection<AuditoriaItem> Eventos { get; } = new();

    public AuditoriaViewModel(AuditoriaService auditoriaService, AuthService authService)
    {
        _auditoriaService = auditoriaService;
        _authService = authService;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        var items = new List<AuditoriaItem>();

        foreach (var evento in await _auditoriaService.ObtenerTodosAsync())
        {
            items.Add(new AuditoriaItem
            {
                FechaHora = evento.FechaHora,
                Usuario = evento.Usuario.NombreCompleto,
                Accion = $"{evento.Accion} ({evento.TipoEntidad})",
                Detalle = evento.Detalle
            });
        }

        foreach (var acceso in await _authService.ObtenerHistorialAccesosAsync())
        {
            items.Add(new AuditoriaItem
            {
                FechaHora = acceso.FechaHora,
                Usuario = acceso.Usuario.NombreCompleto,
                Accion = acceso.TipoEvento.ToString(),
                Detalle = acceso.EquipoOrigen ?? string.Empty
            });
        }

        Eventos.Clear();
        foreach (var item in items.OrderByDescending(i => i.FechaHora))
        {
            Eventos.Add(item);
        }
    }
}
