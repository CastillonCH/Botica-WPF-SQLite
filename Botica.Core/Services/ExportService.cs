using System.Reflection;
using System.Text;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Botica.Core.Services;

public static class ExportService
{
    public static void ExportarExcel<T>(IEnumerable<T> datos, string rutaArchivo, string nombreHoja = "Datos")
    {
        var propiedades = ObtenerPropiedadesSimples<T>();

        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add(nombreHoja);

        for (var i = 0; i < propiedades.Length; i++)
        {
            hoja.Cell(1, i + 1).Value = propiedades[i].Name;
        }

        var fila = 2;
        foreach (var item in datos)
        {
            for (var i = 0; i < propiedades.Length; i++)
            {
                hoja.Cell(fila, i + 1).Value = propiedades[i].GetValue(item)?.ToString() ?? string.Empty;
            }

            fila++;
        }

        hoja.Columns().AdjustToContents();
        libro.SaveAs(rutaArchivo);
    }

    public static void ExportarCsv<T>(IEnumerable<T> datos, string rutaArchivo)
    {
        var propiedades = ObtenerPropiedadesSimples<T>();

        using var writer = new StreamWriter(rutaArchivo, false, Encoding.UTF8);
        writer.WriteLine(string.Join(",", propiedades.Select(p => EscaparCsv(p.Name))));

        foreach (var item in datos)
        {
            writer.WriteLine(string.Join(",",
                propiedades.Select(p => EscaparCsv(p.GetValue(item)?.ToString() ?? string.Empty))));
        }
    }

    public static void ExportarPdf<T>(IEnumerable<T> datos, string rutaArchivo, string titulo)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var propiedades = ObtenerPropiedadesSimples<T>();
        var lista = datos.ToList();

        Document.Create(contenedor =>
        {
            contenedor.Page(pagina =>
            {
                pagina.Margin(30);
                pagina.Header().Text(titulo).FontSize(16).Bold();
                pagina.Content().Table(tabla =>
                {
                    tabla.ColumnsDefinition(columnas =>
                    {
                        foreach (var _ in propiedades)
                        {
                            columnas.RelativeColumn();
                        }
                    });

                    foreach (var propiedad in propiedades)
                    {
                        tabla.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text(propiedad.Name).Bold();
                    }

                    foreach (var item in lista)
                    {
                        foreach (var propiedad in propiedades)
                        {
                            tabla.Cell().Padding(4).Text(propiedad.GetValue(item)?.ToString() ?? string.Empty);
                        }
                    }
                });
                pagina.Footer().AlignCenter().Text(texto =>
                {
                    texto.CurrentPageNumber();
                    texto.Span(" / ");
                    texto.TotalPages();
                });
            });
        }).GeneratePdf(rutaArchivo);
    }

    private static PropertyInfo[] ObtenerPropiedadesSimples<T>()
    {
        return typeof(T).GetProperties()
            .Where(p => EsTipoSimple(p.PropertyType))
            .ToArray();
    }

    private static bool EsTipoSimple(Type tipo)
    {
        var tipoBase = Nullable.GetUnderlyingType(tipo) ?? tipo;
        return tipoBase.IsPrimitive || tipoBase.IsEnum ||
               tipoBase == typeof(string) || tipoBase == typeof(decimal) ||
               tipoBase == typeof(DateTime) || tipoBase == typeof(Guid);
    }

    private static string EscaparCsv(string valor)
    {
        if (valor.Contains(',') || valor.Contains('"') || valor.Contains('\n'))
        {
            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        }

        return valor;
    }
}
