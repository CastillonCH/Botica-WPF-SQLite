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
    }
}
