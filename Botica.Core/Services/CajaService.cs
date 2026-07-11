using Botica.Core.Data;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class CajaService
{
    private readonly BoticaDbContext _dbContext;

    public CajaService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Caja?> ObtenerCajaAbiertaAsync()
    {
        return _dbContext.Cajas.FirstOrDefaultAsync(c => c.Estado == EstadoCaja.Abierta);
    }

    public async Task<Caja> AbrirCajaAsync(int usuarioId, decimal montoApertura)
    {
        var caja = new Caja
        {
            UsuarioAperturaId = usuarioId,
            MontoApertura = montoApertura,
            Estado = EstadoCaja.Abierta
        };

        _dbContext.Cajas.Add(caja);
        await _dbContext.SaveChangesAsync();

        return caja;
    }

    public async Task RegistrarMovimientoAsync(int cajaId, TipoMovimientoCaja tipo, string concepto, decimal monto,
        int usuarioId)
    {
        _dbContext.MovimientosCaja.Add(new MovimientoCaja
        {
            CajaId = cajaId,
            Tipo = tipo,
            Concepto = concepto,
            Monto = monto,
            UsuarioId = usuarioId
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task<decimal> CalcularMontoSistemaAsync(int cajaId)
    {
        var caja = await _dbContext.Cajas.FirstAsync(c => c.Id == cajaId);

        var ingresos = await _dbContext.MovimientosCaja
            .Where(m => m.CajaId == cajaId && m.Tipo == TipoMovimientoCaja.Ingreso)
            .SumAsync(m => m.Monto);

        var egresos = await _dbContext.MovimientosCaja
            .Where(m => m.CajaId == cajaId && m.Tipo == TipoMovimientoCaja.Egreso)
            .SumAsync(m => m.Monto);

        var ventasEfectivo = await _dbContext.Ventas
            .Where(v => v.CajaId == cajaId && v.Estado == EstadoVenta.Emitida && v.MetodoPago == MetodoPago.Efectivo)
            .SumAsync(v => v.Total);

        return caja.MontoApertura + ingresos - egresos + ventasEfectivo;
    }

    public async Task<Caja> CerrarCajaAsync(int cajaId, int usuarioCierreId, decimal montoDeclarado)
    {
        var caja = await _dbContext.Cajas.FirstAsync(c => c.Id == cajaId);
        var montoSistema = await CalcularMontoSistemaAsync(cajaId);

        caja.MontoCierreSistema = montoSistema;
        caja.MontoCierreDeclarado = montoDeclarado;
        caja.UsuarioCierreId = usuarioCierreId;
        caja.FechaCierre = DateTime.Now;
        caja.Estado = EstadoCaja.Cerrada;

        await _dbContext.SaveChangesAsync();

        return caja;
    }

    public async Task<List<MovimientoCaja>> ObtenerMovimientosAsync(int cajaId)
    {
        return await _dbContext.MovimientosCaja
            .Where(m => m.CajaId == cajaId)
            .OrderByDescending(m => m.FechaHora)
            .ToListAsync();
    }
}
