using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class BackupsWindow : Window
{
    private readonly BackupsViewModel _viewModel;

    public BackupsWindow(BackupsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        Loaded += (_, _) => viewModel.CargarCommand.Execute(null);
    }

    private async void RestaurarButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.RespaldoSeleccionado is null)
        {
            MessageBox.Show(this, "Seleccione un respaldo de la lista.", "Restaurar respaldo");
            return;
        }

        var resultado = MessageBox.Show(this,
            "Esto reemplazará todos los datos actuales con los del respaldo seleccionado. ¿Desea continuar?",
            "Confirmar restauración", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (resultado != MessageBoxResult.Yes)
        {
            return;
        }

        await _viewModel.RestaurarRespaldoCommand.ExecuteAsync(null);
    }
}
