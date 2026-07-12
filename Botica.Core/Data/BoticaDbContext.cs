using Botica.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Botica.Core.Data;

public class BoticaDbContext : DbContext
{
    public BoticaDbContext(DbContextOptions<BoticaDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<HistorialAcceso> HistorialAccesos => Set<HistorialAcceso>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Laboratorio> Laboratorios => Set<Laboratorio>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Caja> Cajas => Set<Caja>();
    public DbSet<MovimientoCaja> MovimientosCaja => Set<MovimientoCaja>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<DetalleCompra> DetallesCompra => Set<DetalleCompra>();
    public DbSet<ConfiguracionSistema> ConfiguracionSistema => Set<ConfiguracionSistema>();
    public DbSet<AuditoriaEvento> AuditoriaEventos => Set<AuditoriaEvento>();
    public DbSet<AjusteInventario> AjustesInventario => Set<AjusteInventario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(u => u.NombreUsuario).IsUnique();
        });

        modelBuilder.Entity<HistorialAcceso>(entity =>
        {
            entity.HasOne(h => h.Usuario)
                .WithMany(u => u.HistorialAccesos)
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        modelBuilder.Entity<Laboratorio>(entity =>
        {
            entity.HasIndex(l => l.Nombre).IsUnique();
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasIndex(p => p.Codigo).IsUnique();
            entity.HasIndex(p => p.CodigoBarras).IsUnique().HasFilter("[CodigoBarras] IS NOT NULL");
            entity.Property(p => p.PrecioCompra).HasPrecision(10, 2);
            entity.Property(p => p.PrecioVenta).HasPrecision(10, 2);

            entity.HasOne(p => p.Laboratorio)
                .WithMany(l => l.Productos)
                .HasForeignKey(p => p.LaboratorioId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasIndex(c => c.NumeroDocumento);
        });

        modelBuilder.Entity<Caja>(entity =>
        {
            entity.Property(c => c.MontoApertura).HasPrecision(10, 2);
            entity.Property(c => c.MontoCierreDeclarado).HasPrecision(10, 2);
            entity.Property(c => c.MontoCierreSistema).HasPrecision(10, 2);

            entity.HasOne(c => c.UsuarioApertura)
                .WithMany()
                .HasForeignKey(c => c.UsuarioAperturaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.UsuarioCierre)
                .WithMany()
                .HasForeignKey(c => c.UsuarioCierreId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MovimientoCaja>(entity =>
        {
            entity.Property(m => m.Monto).HasPrecision(10, 2);

            entity.HasOne(m => m.Caja)
                .WithMany(c => c.Movimientos)
                .HasForeignKey(m => m.CajaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.Property(v => v.Subtotal).HasPrecision(10, 2);
            entity.Property(v => v.Descuento).HasPrecision(10, 2);
            entity.Property(v => v.Igv).HasPrecision(10, 2);
            entity.Property(v => v.Total).HasPrecision(10, 2);
            entity.Property(v => v.MontoRecibido).HasPrecision(10, 2);
            entity.Property(v => v.Vuelto).HasPrecision(10, 2);

            entity.HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Caja)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.CajaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.Property(d => d.PrecioUnitario).HasPrecision(10, 2);
            entity.Property(d => d.Descuento).HasPrecision(10, 2);
            entity.Property(d => d.Subtotal).HasPrecision(10, 2);

            entity.HasOne(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Producto)
                .WithMany(p => p.DetallesVenta)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.HasOne(l => l.Producto)
                .WithMany(p => p.Lotes)
                .HasForeignKey(l => l.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.Property(c => c.Total).HasPrecision(10, 2);

            entity.HasOne(c => c.Proveedor)
                .WithMany(p => p.Compras)
                .HasForeignKey(c => c.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DetalleCompra>(entity =>
        {
            entity.Property(d => d.PrecioUnitario).HasPrecision(10, 2);
            entity.Property(d => d.Subtotal).HasPrecision(10, 2);

            entity.HasOne(d => d.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.CompraId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ConfiguracionSistema>(entity =>
        {
            entity.Property(c => c.TasaIgv).HasPrecision(5, 4);
        });

        modelBuilder.Entity<AuditoriaEvento>(entity =>
        {
            entity.HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AjusteInventario>(entity =>
        {
            entity.HasOne(a => a.Producto)
                .WithMany()
                .HasForeignKey(a => a.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
