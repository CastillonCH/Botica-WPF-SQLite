using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class LaboratoriosViewModel : ObservableObject
{
    private readonly LaboratorioService _service;

    public ObservableCollection<Laboratorio> Laboratorios { get; } = new();

    [ObservableProperty]
    private Laboratorio laboratorioSeleccionado = new();

    [ObservableProperty]
    private string mensaje = string.Empty;

    public LaboratoriosViewModel(LaboratorioService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        Laboratorios.Clear();
        foreach (var laboratorio in await _service.ObtenerTodosAsync())
        {
            Laboratorios.Add(laboratorio);
        }
    }

    [RelayCommand]
    private void Nuevo()
    {
        LaboratorioSeleccionado = new Laboratorio();
        Mensaje = string.Empty;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(LaboratorioSeleccionado.Nombre))
        {
            Mensaje = "El nombre es obligatorio.";
            return;
        }

        try
        {
            await _service.GuardarAsync(LaboratorioSeleccionado);
            Mensaje = string.Empty;
            await CargarAsync();
            Nuevo();
        }
        catch (Exception)
        {
            Mensaje = "Ya existe un laboratorio con ese nombre.";
        }
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (LaboratorioSeleccionado.Id == 0)
        {
            return;
        }

        var eliminado = await _service.EliminarAsync(LaboratorioSeleccionado.Id);
        Mensaje = eliminado ? string.Empty : "No se puede eliminar: tiene productos asociados.";
        await CargarAsync();
        Nuevo();
    }
}
