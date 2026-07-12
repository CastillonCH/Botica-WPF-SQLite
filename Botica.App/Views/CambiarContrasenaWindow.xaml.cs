using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class CambiarContrasenaWindow : Window
{
    private readonly CambiarContrasenaViewModel _viewModel;

    public CambiarContrasenaWindow(CambiarContrasenaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        _viewModel.CambioExitoso += (_, _) =>
        {
            DialogResult = true;
            Close();
        };
    }

    private async void GuardarButton_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.GuardarCommand.ExecuteAsync(new object[]
        {
            ActualPasswordBox.Password,
            NuevaPasswordBox.Password,
            ConfirmarPasswordBox.Password
        });
    }
}
