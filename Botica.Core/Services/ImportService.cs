using Botica.Core.Data;
using Botica.Core.Entities;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class ImportService
{
    private readonly BoticaDbContext _dbContext;

    public ImportService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> ImportarCategoriasAsync(string rutaArchivo)
    {
        using var libro = new XLWorkbook(rutaArchivo);
        var hoja = libro.Worksheet(1);
        var importados = 0;

        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            var nombre = fila.Cell(1).GetString().Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                continue;
            }

            var existente = await _dbContext.Categorias.FirstOrDefaultAsync(c => c.Nombre == nombre);
            if (existente is not null)
            {
                continue;
            }

            _dbContext.Categorias.Add(new Categoria
            {
                Nombre = nombre,
                Descripcion = fila.Cell(2).GetString().Trim()
            });
            importados++;
        }

        await _dbContext.SaveChangesAsync();
        return importados;
    }

    public async Task<int> ImportarLaboratoriosAsync(string rutaArchivo)
    {
        using var libro = new XLWorkbook(rutaArchivo);
        var hoja = libro.Worksheet(1);
        var importados = 0;

        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            var nombre = fila.Cell(1).GetString().Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                continue;
            }

            var existente = await _dbContext.Laboratorios.FirstOrDefaultAsync(l => l.Nombre == nombre);
            if (existente is not null)
            {
                continue;
            }

            _dbContext.Laboratorios.Add(new Laboratorio { Nombre = nombre });
            importados++;
        }

        await _dbContext.SaveChangesAsync();
        return importados;
    }

    public async Task<int> ImportarProveedoresAsync(string rutaArchivo)
    {
        using var libro = new XLWorkbook(rutaArchivo);
        var hoja = libro.Worksheet(1);
        var importados = 0;

        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            var razonSocial = fila.Cell(1).GetString().Trim();
            if (string.IsNullOrWhiteSpace(razonSocial))
            {
                continue;
            }

            _dbContext.Proveedores.Add(new Proveedor
            {
                RazonSocial = razonSocial,
                Ruc = fila.Cell(2).GetString().Trim(),
                Direccion = fila.Cell(3).GetString().Trim(),
                Telefono = fila.Cell(4).GetString().Trim()
            });
            importados++;
        }

        await _dbContext.SaveChangesAsync();
        return importados;
    }

    public async Task<int> ImportarProductosAsync(string rutaArchivo)
    {
        // Columnas esperadas: Codigo, Nombre, PrecioCompra, PrecioVenta, StockMinimo, StockActual
        using var libro = new XLWorkbook(rutaArchivo);
        var hoja = libro.Worksheet(1);
        var importados = 0;

        foreach (var fila in hoja.RowsUsed().Skip(1))
        {
            var codigo = fila.Cell(1).GetString().Trim();
            var nombre = fila.Cell(2).GetString().Trim();
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nombre))
            {
                continue;
            }

            var existente = await _dbContext.Productos.FirstOrDefaultAsync(p => p.Codigo == codigo);
            if (existente is not null)
            {
                continue;
            }

            _dbContext.Productos.Add(new Producto
            {
                Codigo = codigo,
                Nombre = nombre,
                PrecioCompra = fila.Cell(3).GetValue<decimal>(),
                PrecioVenta = fila.Cell(4).GetValue<decimal>(),
                StockMinimo = fila.Cell(5).GetValue<int>(),
                StockActual = fila.Cell(6).GetValue<int>()
            });
            importados++;
        }

        await _dbContext.SaveChangesAsync();
        return importados;
    }
}
