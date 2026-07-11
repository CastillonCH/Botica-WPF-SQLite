using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class CajaViewModel : ObservableObject
{
    private readonly CajaService _service;
    private int _usuarioId;

    public ObservableCollection<MovimientoCaja> Movimientos { get; } = new();

    [ObservableProperty]
    private Caja? cajaActual;

    [ObservableProperty]
    private decimal montoAperturaNuevo;

    [ObservableProperty]
    private string conceptoMovimiento = string.Empty;

    [ObservableProperty]
    private decimal montoMovimiento;

    [ObservableProperty]
    private decimal montoDeclaradoCierre;

    [ObservableProperty]
    private decimal? montoSistemaCalculado;

    [ObservableProperty]
    private string mensaje = string.Empty;

    public bool HayCajaAbierta => CajaActual is not null;
    public bool NoHayCajaAbierta => CajaActual is null;

    public CajaViewModel(CajaService service)
    {
        _service = service;
    }

    public void EstablecerUsuario(int usuarioId) => _usuarioId = usuarioId;

    partial void OnCajaActualChanged(Caja? value)
    {
        OnPropertyChanged(nameof(HayCajaAbierta));
        OnPropertyChanged(nameof(NoHayCajaAbierta));
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        CajaActual = await _service.ObtenerCajaAbiertaAsync();
        Mensaje = string.Empty;

        if (CajaActual is not null)
        {
            await CargarMovimientosAsync();
        }
    }

    private async Task CargarMovimientosAsync()
    {
        Movimientos.Clear();
        foreach (var movimiento in await _service.ObtenerMovimientosAsync(CajaActual!.Id))
        {
            Movimientos.Add(movimiento);
        }
    }

    [RelayCommand]
    private async Task AbrirCajaAsync()
    {
        if (MontoAperturaNuevo < 0)
        {
            Mensaje = "El monto de apertura no puede ser negativo.";
            return;
        }

        CajaActual = await _service.AbrirCajaAsync(_usuarioId, MontoAperturaNuevo);
        MontoAperturaNuevo = 0;
        Mensaje = string.Empty;
        await CargarMovimientosAsync();
    }

    [RelayCommand]
    private async Task RegistrarIngresoAsync() => await RegistrarMovimientoAsync(TipoMovimientoCaja.Ingreso);

    [RelayCommand]
    private async Task RegistrarEgresoAsync() => await RegistrarMovimientoAsync(TipoMovimientoCaja.Egreso);

    private async Task RegistrarMovimientoAsync(TipoMovimientoCaja tipo)
    {
        if (CajaActual is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ConceptoMovimiento) || MontoMovimiento <= 0)
        {
            Mensaje = "Ingrese un concepto y un monto válido.";
            return;
        }

        await _service.RegistrarMovimientoAsync(CajaActual.Id, tipo, ConceptoMovimiento, MontoMovimiento, _usuarioId);
        ConceptoMovimiento = string.Empty;
        MontoMovimiento = 0;
        Mensaje = string.Empty;
        await CargarMovimientosAsync();
    }

    [RelayCommand]
    private async Task CalcularCierreAsync()
    {
        if (CajaActual is null)
        {
            return;
        }

        MontoSistemaCalculado = await _service.CalcularMontoSistemaAsync(CajaActual.Id);
    }

    [RelayCommand]
    private async Task CerrarCajaAsync()
    {
        if (CajaActual is null)
        {
            return;
        }

        await _service.CerrarCajaAsync(CajaActual.Id, _usuarioId, MontoDeclaradoCierre);
        Mensaje = "Caja cerrada correctamente.";
        MontoDeclaradoCierre = 0;
        MontoSistemaCalculado = null;
        await CargarAsync();
    }
}
