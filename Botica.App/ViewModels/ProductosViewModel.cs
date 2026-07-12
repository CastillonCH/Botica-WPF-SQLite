using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class ProductosViewModel : ObservableObject
{
    private readonly ProductoService _productoService;
    private readonly CategoriaService _categoriaService;
    private readonly LaboratorioService _laboratorioService;
    private int _usuarioId;

    public ObservableCollection<Producto> Productos { get; } = new();
    public ObservableCollection<Categoria> CategoriasDisponibles { get; } = new();
    public ObservableCollection<Laboratorio> LaboratoriosDisponibles { get; } = new();

    [ObservableProperty]
    private Producto productoSeleccionado = new();

    [ObservableProperty]
    private string textoBusqueda = string.Empty;

    [ObservableProperty]
    private string mensaje = string.Empty;

    public ProductosViewModel(ProductoService productoService, CategoriaService categoriaService,
        LaboratorioService laboratorioService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
        _laboratorioService = laboratorioService;
    }

    public void EstablecerUsuario(int usuarioId) => _usuarioId = usuarioId;

    [RelayCommand]
    private async Task CargarAsync()
    {
        CategoriasDisponibles.Clear();
        foreach (var categoria in await _categoriaService.ObtenerTodosAsync())
        {
            CategoriasDisponibles.Add(categoria);
        }

        LaboratoriosDisponibles.Clear();
        foreach (var laboratorio in await _laboratorioService.ObtenerTodosAsync())
        {
            LaboratoriosDisponibles.Add(laboratorio);
        }

        await BuscarAsync();
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        Productos.Clear();
        foreach (var producto in await _productoService.ObtenerTodosAsync(TextoBusqueda))
        {
            Productos.Add(producto);
        }
    }

    [RelayCommand]
    private void Nuevo()
    {
        ProductoSeleccionado = new Producto();
        Mensaje = string.Empty;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(ProductoSeleccionado.Codigo) ||
            string.IsNullOrWhiteSpace(ProductoSeleccionado.Nombre))
        {
            Mensaje = "Código y nombre son obligatorios.";
            return;
        }

        try
        {
            await _productoService.GuardarAsync(ProductoSeleccionado, _usuarioId);
            Mensaje = string.Empty;
            await BuscarAsync();
            Nuevo();
        }
        catch (Exception)
        {
            Mensaje = "Ya existe un producto con ese código o código de barras.";
        }
    }

    [RelayCommand]
    private async Task EliminarAsync()
    {
        if (ProductoSeleccionado.Id == 0)
        {
            return;
        }

        var eliminado = await _productoService.EliminarAsync(ProductoSeleccionado.Id, _usuarioId);
        Mensaje = eliminado ? string.Empty : "No se puede eliminar: tiene ventas registradas. Desactívelo en su lugar.";
        await BuscarAsync();
        Nuevo();
    }

    [RelayCommand]
    private async Task CambiarEstadoAsync()
    {
        if (ProductoSeleccionado.Id == 0)
        {
            return;
        }

        await _productoService.CambiarEstadoAsync(ProductoSeleccionado.Id, _usuarioId);
        await BuscarAsync();
        Nuevo();
    }
}
