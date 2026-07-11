using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class ProveedoresViewModel : ObservableObject
{
    private readonly ProveedorService _service;

    public ObservableCollection<Proveedor> Proveedores { get; } = new();

    [ObservableProperty]
    private Proveedor proveedorSeleccionado = new();

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private string mensaje = string.Empty;

    public ProveedoresViewModel(ProveedorService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        Proveedores.Clear();
        foreach (var proveedor in await _service.ObtenerTodosAsync(TextoBusqueda))
        {
            Proveedores.Add(proveedor);
        }
    }

    [RelayCommand]
    private void Nuevo()
    {
        ProveedorSeleccionado = new Proveedor();
        Mensaje = string.Empty;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(ProveedorSeleccionado.RazonSocial))
        {
            Mensaje = "La razón social es obligatoria.";
            return;
        }

        await _service.GuardarAsync(ProveedorSeleccionado);
        Mensaje = string.Empty;
        await CargarAsync();
        Nuevo();
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (ProveedorSeleccionado.Id == 0)
        {
            return;
        }

        var eliminado = await _service.EliminarAsync(ProveedorSeleccionado.Id);
        Mensaje = eliminado ? string.Empty : "No se puede eliminar: tiene compras registradas.";
        await CargarAsync();
        Nuevo();
    }
}
