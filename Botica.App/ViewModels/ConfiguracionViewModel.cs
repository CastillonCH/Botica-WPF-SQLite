using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class ConfiguracionViewModel : ObservableObject
{
    private readonly ConfiguracionService _service;

    [ObservableProperty]
    private ConfiguracionSistema configuracion = new();

    [ObservableProperty]
    private string mensaje = string.Empty;

    public ConfiguracionViewModel(ConfiguracionService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        Configuracion = await _service.ObtenerAsync();
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        await _service.GuardarAsync(Configuracion);
        Mensaje = "Configuración guardada.";
    }
}
