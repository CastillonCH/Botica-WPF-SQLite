using System.Collections.ObjectModel;
using Botica.Core.Entities;
using Botica.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Botica.App.ViewModels;

public partial class VencimientosViewModel : ObservableObject
{
    private readonly LoteService _service;

    public ObservableCollection<Lote> ProximosAVencer { get; } = new();
    public ObservableCollection<Lote> Vencidos { get; } = new();

    [ObservableProperty]
    private int diasAnticipacion = 90;

    public VencimientosViewModel(LoteService service)
    {
        _service = service;
    }

    [RelayCommand]
    private async Task CargarAsync()
    {
        ProximosAVencer.Clear();
        foreach (var lote in await _service.ObtenerProximosAVencerAsync(DiasAnticipacion))
        {
            ProximosAVencer.Add(lote);
        }

        Vencidos.Clear();
        foreach (var lote in await _service.ObtenerVencidosAsync())
        {
            Vencidos.Add(lote);
        }
    }
}
