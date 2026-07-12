using Botica.Core.Data;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class InventarioService
{
    private readonly BoticaDbContext _dbContext;
    private readonly AuditoriaService _auditoriaService;

    public InventarioService(BoticaDbContext dbContext, AuditoriaService auditoriaService)
    {
        _dbContext = dbContext;
        _auditoriaService = auditoriaService;
    }

    public async Task<string?> RegistrarAjusteAsync(int productoId, TipoAjusteInventario tipo, int cantidad,
        string motivo, int usuarioId)
    {
        if (cantidad <= 0)
        {
            return "La cantidad debe ser mayor a cero.";
        }

        var producto = await _dbContext.Productos.FindAsync(productoId);
        if (producto is null)
        {
            return "Producto no encontrado.";
        }

        if (tipo == TipoAjusteInventario.Salida && producto.StockActual < cantidad)
        {
            return "Stock insuficiente para registrar la salida.";
        }

        producto.StockActual += tipo == TipoAjusteInventario.Entrada ? cantidad : -cantidad;

        _dbContext.AjustesInventario.Add(new AjusteInventario
        {
            ProductoId = productoId,
            Tipo = tipo,
            Cantidad = cantidad,
            Motivo = motivo,
            UsuarioId = usuarioId
        });

        await _dbContext.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(usuarioId, "Producto", productoId, AccionAuditoria.Modificacion,
            $"Ajuste de inventario ({tipo}): {cantidad}. Motivo: {motivo}");

        return null;
    }

    public async Task<List<MovimientoKardex>> ObtenerKardexAsync(int productoId)
    {
        var movimientos = new List<MovimientoKardex>();

        var entradasCompra = await _dbContext.DetallesCompra
            .Include(d => d.Compra)
            .Where(d => d.ProductoId == productoId)
            .Select(d => new MovimientoKardex
            {
                FechaHora = d.Compra.FechaCompra,
                TipoMovimiento = "Compra",
                Entrada = d.Cantidad,
                Salida = 0,
                Detalle = $"N° comprobante: {d.Compra.NumeroComprobante}"
            })
            .ToListAsync();
        movimientos.AddRange(entradasCompra);

        var salidasVenta = await _dbContext.DetallesVenta
            .Include(d => d.Venta)
            .Where(d => d.ProductoId == productoId && d.Venta.Estado == EstadoVenta.Emitida)
            .Select(d => new MovimientoKardex
            {
                FechaHora = d.Venta.FechaHora,
                TipoMovimiento = "Venta",
                Entrada = 0,
                Salida = d.Cantidad,
                Detalle = $"Venta #{d.VentaId}"
            })
            .ToListAsync();
        movimientos.AddRange(salidasVenta);

        var ajustes = await _dbContext.AjustesInventario
            .Where(a => a.ProductoId == productoId)
            .Select(a => new MovimientoKardex
            {
                FechaHora = a.FechaHora,
                TipoMovimiento = $"Ajuste ({a.Tipo})",
                Entrada = a.Tipo == TipoAjusteInventario.Entrada ? a.Cantidad : 0,
                Salida = a.Tipo == TipoAjusteInventario.Salida ? a.Cantidad : 0,
                Detalle = a.Motivo
            })
            .ToListAsync();
        movimientos.AddRange(ajustes);

        var ordenados = movimientos.OrderBy(m => m.FechaHora).ToList();

        var saldo = 0;
        foreach (var movimiento in ordenados)
        {
            saldo += movimiento.Entrada - movimiento.Salida;
            movimiento.Saldo = saldo;
        }

        ordenados.Reverse();
        return ordenados;
    }
}
