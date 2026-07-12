using System.Windows;
using Botica.App.ViewModels;
using Botica.Core.Services;
using Microsoft.Win32;

namespace Botica.App.Views;

public partial class ReportesWindow : Window
{
    public ReportesViewModel ViewModel { get; }

    public ReportesWindow(ReportesViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
        Loaded += async (_, _) =>
        {
            await viewModel.GenerarVentasComprasCommand.ExecuteAsync(null);
            await viewModel.CargarInventarioCommand.ExecuteAsync(null);
        };
    }

    private void ExportarVentasPorProducto_Click(object sender, RoutedEventArgs e) =>
        Exportar(ViewModel.VentasPorProducto, "VentasPorProducto");

    private void ExportarVentasPorUsuario_Click(object sender, RoutedEventArgs e) =>
        Exportar(ViewModel.VentasPorUsuario, "VentasPorUsuario");

    private void ExportarComprasPorProveedor_Click(object sender, RoutedEventArgs e) =>
        Exportar(ViewModel.ComprasPorProveedor, "ComprasPorProveedor");

    private void ExportarGananciasPorDia_Click(object sender, RoutedEventArgs e) =>
        Exportar(ViewModel.GananciasPorDia, "GananciasPorDia");

    private void Exportar<T>(IEnumerable<T> datos, string nombreSugerido)
    {
        var dialogo = new SaveFileDialog
        {
            FileName = nombreSugerido,
            Filter = "Excel (*.xlsx)|*.xlsx|CSV (*.csv)|*.csv|PDF (*.pdf)|*.pdf"
        };

        if (dialogo.ShowDialog() != true)
        {
            return;
        }

        var ruta = dialogo.FileName;

        if (ruta.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            ExportService.ExportarCsv(datos, ruta);
        }
        else if (ruta.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            ExportService.ExportarPdf(datos, ruta, nombreSugerido);
        }
        else
        {
            ExportService.ExportarExcel(datos, ruta, nombreSugerido);
        }

        MessageBox.Show(this, "Exportación completada.", "Reportes");
    }
}
