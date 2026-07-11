using System.Windows;
using Botica.App.ViewModels;
using Botica.App.Views;
using Botica.Core.Data;
using Botica.Core.Entities;
using Botica.Core.Enums;
using Botica.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Botica.App;

public partial class App : Application
{
    private ServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigurarServicios(services);
        _serviceProvider = services.BuildServiceProvider();

        var dbContext = _serviceProvider.GetRequiredService<BoticaDbContext>();
        dbContext.Database.Migrate();
        SembrarUsuarioAdministrador(dbContext);

        MostrarLogin();
    }

    private static void ConfigurarServicios(ServiceCollection services)
    {
        services.AddDbContext<BoticaDbContext>(options =>
            options.UseSqlite(BoticaDbPath.GetDefaultConnectionString()), ServiceLifetime.Singleton);

        services.AddSingleton<AuthService>();
        services.AddSingleton<CategoriaService>();
        services.AddSingleton<LaboratorioService>();
        services.AddSingleton<ProductoService>();
        services.AddSingleton<CajaService>();
        services.AddSingleton<VentaService>();
        services.AddSingleton<ProveedorService>();
        services.AddSingleton<LoteService>();
        services.AddSingleton<CompraService>();
        services.AddSingleton<DashboardService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginWindow>();
        services.AddTransient<CategoriasViewModel>();
        services.AddTransient<CategoriasWindow>();
        services.AddTransient<LaboratoriosViewModel>();
        services.AddTransient<LaboratoriosWindow>();
        services.AddTransient<ProductosViewModel>();
        services.AddTransient<ProductosWindow>();
        services.AddTransient<CajaViewModel>();
        services.AddTransient<CajaWindow>();
        services.AddTransient<VentasViewModel>();
        services.AddTransient<VentasWindow>();
        services.AddTransient<ProveedoresViewModel>();
        services.AddTransient<ProveedoresWindow>();
        services.AddTransient<ComprasViewModel>();
        services.AddTransient<ComprasWindow>();
        services.AddTransient<VencimientosViewModel>();
        services.AddTransient<VencimientosWindow>();
        services.AddTransient<DashboardViewModel>();
    }

    private static void SembrarUsuarioAdministrador(BoticaDbContext dbContext)
    {
        if (dbContext.Usuarios.Any())
        {
            return;
        }

        dbContext.Usuarios.Add(new Usuario
        {
            NombreUsuario = "admin",
            PasswordHash = PasswordHasher.Hash("admin123"),
            NombreCompleto = "Administrador",
            Rol = RolUsuario.Administrador,
            Activo = true
        });
        dbContext.SaveChanges();
    }

    private void MostrarLogin()
    {
        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        var resultado = loginWindow.ShowDialog();

        if (resultado != true)
        {
            Shutdown();
            return;
        }

        var usuario = ((LoginViewModel)loginWindow.DataContext).UsuarioAutenticado!;
        var mainWindow = new MainWindow(usuario, _serviceProvider.GetRequiredService<AuthService>(), _serviceProvider);
        mainWindow.SesionCerrada += (_, _) => MostrarLogin();
        mainWindow.AppCerrandose += (_, _) => Shutdown();
        mainWindow.Show();
    }
}
