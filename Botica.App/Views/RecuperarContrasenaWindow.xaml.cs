using System.Windows;
using Botica.App.ViewModels;

namespace Botica.App.Views;

public partial class RecuperarContrasenaWindow : Window
{
    public RecuperarContrasenaWindow(RecuperarContrasenaViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.RestablecimientoExitoso += (_, _) => DialogResult = true;
    }
}
