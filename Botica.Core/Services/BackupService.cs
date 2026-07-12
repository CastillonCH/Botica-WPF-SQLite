using Botica.Core.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Services;

public class BackupService
{
    private readonly BoticaDbContext _dbContext;

    public BackupService(BoticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public static string ObtenerCarpetaRespaldos()
    {
        var carpeta = Path.Combine(
            Path.GetDirectoryName(BoticaDbPath.GetDefaultDbPath())!,
            "Backups");

        Directory.CreateDirectory(carpeta);

        return carpeta;
    }

    public async Task<string> CrearRespaldoAsync()
    {
        var nombreArchivo = $"botica_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
        var rutaDestino = Path.Combine(ObtenerCarpetaRespaldos(), nombreArchivo);

        var origen = (SqliteConnection)_dbContext.Database.GetDbConnection();
        var estabaCerrada = origen.State != System.Data.ConnectionState.Open;
        if (estabaCerrada)
        {
            await origen.OpenAsync();
        }

        using var destino = new SqliteConnection($"Data Source={rutaDestino}");
        destino.Open();
        origen.BackupDatabase(destino);

        if (estabaCerrada)
        {
            await origen.CloseAsync();
        }

        return rutaDestino;
    }

    public List<string> ObtenerRespaldosDisponibles()
    {
        return Directory.GetFiles(ObtenerCarpetaRespaldos(), "*.db")
            .OrderByDescending(f => f)
            .ToList();
    }

    public async Task RestaurarRespaldoAsync(string rutaRespaldo)
    {
        var destino = (SqliteConnection)_dbContext.Database.GetDbConnection();
        var estabaCerrada = destino.State != System.Data.ConnectionState.Open;
        if (estabaCerrada)
        {
            await destino.OpenAsync();
        }

        using var origen = new SqliteConnection($"Data Source={rutaRespaldo}");
        origen.Open();
        origen.BackupDatabase(destino);

        if (estabaCerrada)
        {
            await destino.CloseAsync();
        }
    }
}
