using System.Windows;
using Botica.App.ViewModels;
using Microsoft.Win32;

namespace Botica.App.Views;

public partial class ImportacionWindow : Window
{
    private readonly ImportacionViewModel _viewModel;

    public ImportacionWindow(ImportacionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private async void ImportarCategorias_Click(object sender, RoutedEventArgs e)
    {
        var ruta = ElegirArchivo();
        if (ruta is not null)
        {
            await _viewModel.ImportarCategoriasCommand.ExecuteAsync(ruta);
        }
    }

    private async void ImportarLaboratorios_Click(object sender, RoutedEventArgs e)
    {
        var ruta = ElegirArchivo();
        if (ruta is not null)
        {
            await _viewModel.ImportarLaboratoriosCommand.ExecuteAsync(ruta);
        }
    }

    private async void ImportarProveedores_Click(object sender, RoutedEventArgs e)
    {
        var ruta = ElegirArchivo();
        if (ruta is not null)
        {
            await _viewModel.ImportarProveedoresCommand.ExecuteAsync(ruta);
        }
    }

    private async void ImportarProductos_Click(object sender, RoutedEventArgs e)
    {
        var ruta = ElegirArchivo();
        if (ruta is not null)
        {
            await _viewModel.ImportarProductosCommand.ExecuteAsync(ruta);
        }
    }

    private string? ElegirArchivo()
    {
        var dialogo = new OpenFileDialog { Filter = "Excel (*.xlsx)|*.xlsx" };
        return dialogo.ShowDialog() == true ? dialogo.FileName : null;
    }
}
