namespace Botica.Core.Data;

public static class BoticaDbPath
{
    public static string GetDefaultDbPath()
    {
        var carpeta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Botica");

        Directory.CreateDirectory(carpeta);

        return Path.Combine(carpeta, "botica.db");
    }

    public static string GetDefaultConnectionString() => $"Data Source={GetDefaultDbPath()}";
}
