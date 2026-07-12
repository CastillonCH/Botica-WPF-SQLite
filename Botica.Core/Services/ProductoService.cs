using Botica.Core.Data;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class ProductoService
{
    private readonly BoticaDbContext _dbContext;
    private readonly AuditoriaService _auditoriaService;

    public ProductoService(BoticaDbContext dbContext, AuditoriaService auditoriaService)
    {
        _dbContext = dbContext;
        _auditoriaService = auditoriaService;
    }

    public async Task<List<Producto>> ObtenerTodosAsync(string? textoBusqueda = null)
    {
        var query = _dbContext.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Laboratorio)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(textoBusqueda))
        {
            var texto = textoBusqueda.Trim();
            query = query.Where(p =>
                p.Nombre.Contains(texto) ||
                p.Codigo.Contains(texto) ||
                (p.CodigoBarras != null && p.CodigoBarras.Contains(texto)));
        }

        return await query.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<List<Producto>> BuscarParaVentaAsync(string textoBusqueda)
    {
        if (string.IsNullOrWhiteSpace(textoBusqueda))
        {
            return new List<Producto>();
        }

        var texto = textoBusqueda.Trim();

        return await _dbContext.Productos
            .Where(p => p.Activo && (
                p.Nombre.Contains(texto) ||
                p.Codigo.Contains(texto) ||
                (p.CodigoBarras != null && p.CodigoBarras.Contains(texto))))
            .OrderBy(p => p.Nombre)
            .Take(30)
            .ToListAsync();
    }

    public async Task GuardarAsync(Producto producto, int usuarioId)
    {
        if (producto.Id == 0)
        {
            _dbContext.Productos.Add(producto);
            await _dbContext.SaveChangesAsync();
            await _auditoriaService.RegistrarAsync(usuarioId, "Producto", producto.Id, AccionAuditoria.Creacion,
                $"Producto \"{producto.Nombre}\" registrado.");
            return;
        }

        var entry = _dbContext.Entry(producto);
        var precioAnterior = entry.OriginalValues.GetValue<decimal>(nameof(Producto.PrecioVenta));
        var stockAnterior = entry.OriginalValues.GetValue<int>(nameof(Producto.StockActual));

        _dbContext.Productos.Update(producto);
        await _dbContext.SaveChangesAsync();

        var cambios = new List<string>();
        if (precioAnterior != producto.PrecioVenta)
        {
            cambios.Add($"Precio venta: {precioAnterior:N2} -> {producto.PrecioVenta:N2}");
        }

        if (stockAnterior != producto.StockActual)
        {
            cambios.Add($"Stock: {stockAnterior} -> {producto.StockActual}");
        }

        if (cambios.Count > 0)
        {
            await _auditoriaService.RegistrarAsync(usuarioId, "Producto", producto.Id, AccionAuditoria.Modificacion,
                string.Join("; ", cambios));
        }
    }

    public async Task CambiarEstadoAsync(int id, int usuarioId)
    {
        var producto = await _dbContext.Productos.FindAsync(id);
        if (producto is null)
        {
            return;
        }

        producto.Activo = !producto.Activo;
        await _dbContext.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(usuarioId, "Producto", producto.Id, AccionAuditoria.Modificacion,
            $"Activo cambiado a {producto.Activo}.");
    }

    public async Task<bool> EliminarAsync(int id, int usuarioId)
    {
        var producto = await _dbContext.Productos.FindAsync(id);
        if (producto is null)
        {
            return false;
        }

        var nombre = producto.Nombre;
        _dbContext.Productos.Remove(producto);

        try
        {
            await _dbContext.SaveChangesAsync();
            await _auditoriaService.RegistrarAsync(usuarioId, "Producto", id, AccionAuditoria.Eliminacion,
                $"Producto \"{nombre}\" eliminado.");
            return true;
        }
        catch (DbUpdateException)
        {
            _dbContext.Entry(producto).State = EntityState.Unchanged;
            return false;
        }
    }
}
