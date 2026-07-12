using System.Collections.ObjectModel;
using System.IO;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class BackupsViewModel : ObservableObject
{
    private readonly BackupService _service;

    public ObservableCollection<string> Respaldos { get; } = new();

    [ObservableProperty]
    private string? respaldoSeleccionado;

    [ObservableProperty]
    private string mensaje = string.Empty;

    public BackupsViewModel(BackupService service)
    {
        _service = service;
    }

    [RelayCommand]
    private void Cargar()
    {
        Respaldos.Clear();
        foreach (var respaldo in _service.ObtenerRespaldosDisponibles())
        {
            Respaldos.Add(respaldo);
        }
    }

    [RelayCommand]
    private async Task CrearRespaldoAsync()
    {
        var ruta = await _service.CrearRespaldoAsync();
        Mensaje = $"Respaldo creado: {Path.GetFileName(ruta)}";
        Cargar();
    }

    [RelayCommand]
    private async Task RestaurarRespaldoAsync()
    {
        if (RespaldoSeleccionado is null)
        {
            Mensaje = "Seleccione un respaldo de la lista.";
            return;
        }

        await _service.RestaurarRespaldoAsync(RespaldoSeleccionado);
        Mensaje = "Respaldo restaurado. Reinicie la aplicación para asegurar que todos los módulos reflejen los datos restaurados.";
    }
}
