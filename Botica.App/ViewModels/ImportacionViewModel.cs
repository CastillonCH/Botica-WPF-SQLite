using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class ImportacionViewModel : ObservableObject
{
    private readonly ImportService _service;

    [ObservableProperty]
    private string mensaje = string.Empty;

    public ImportacionViewModel(ImportService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task ImportarCategoriasAsync(string ruta)
    {
        var cantidad = await _service.ImportarCategoriasAsync(ruta);
        Mensaje = $"{cantidad} categorías importadas.";
    }

    [RelayCommand]
    private async Task ImportarLaboratoriosAsync(string ruta)
    {
        var cantidad = await _service.ImportarLaboratoriosAsync(ruta);
        Mensaje = $"{cantidad} laboratorios importados.";
    }

    [RelayCommand]
    private async Task ImportarProveedoresAsync(string ruta)
    {
        var cantidad = await _service.ImportarProveedoresAsync(ruta);
        Mensaje = $"{cantidad} proveedores importados.";
    }

    [RelayCommand]
    private async Task ImportarProductosAsync(string ruta)
    {
        var cantidad = await _service.ImportarProductosAsync(ruta);
        Mensaje = $"{cantidad} productos importados.";
    }
}
