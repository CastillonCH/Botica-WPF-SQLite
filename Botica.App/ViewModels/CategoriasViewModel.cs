using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class CategoriasViewModel : ObservableObject
{
    private readonly CategoriaService _service;

    public ObservableCollection<Categoria> Categorias { get; } = new();

    [ObservableProperty]
    private Categoria categoriaSeleccionada = new();

    [ObservableProperty]
    private string mensaje = string.Empty;

    public CategoriasViewModel(CategoriaService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        Categorias.Clear();
        foreach (var categoria in await _service.ObtenerTodosAsync())
        {
            Categorias.Add(categoria);
        }
    }

    [RelayCommand]
    private void Nuevo()
    {
        CategoriaSeleccionada = new Categoria();
        Mensaje = string.Empty;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(CategoriaSeleccionada.Nombre))
        {
            Mensaje = "El nombre es obligatorio.";
            return;
        }

        try
        {
            await _service.GuardarAsync(CategoriaSeleccionada);
            Mensaje = string.Empty;
            await CargarAsync();
            Nuevo();
        }
        catch (Exception)
        {
            Mensaje = "Ya existe una categoría con ese nombre.";
        }
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (CategoriaSeleccionada.Id == 0)
        {
            return;
        }

        var eliminado = await _service.EliminarAsync(CategoriaSeleccionada.Id);
        Mensaje = eliminado ? string.Empty : "No se puede eliminar: tiene productos asociados.";
        await CargarAsync();
        Nuevo();
    }
}
