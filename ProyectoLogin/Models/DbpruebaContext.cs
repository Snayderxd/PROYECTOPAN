using Microsoft.EntityFrameworkCore;

namespace ProyectoLogin.Models
{
    public class DbpruebaContext : DbContext
    {
        public DbpruebaContext(DbContextOptions<DbpruebaContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<ProductosPan> ProductosPan { get; set; }
        public DbSet<Carrito> Carrito { get; set; }
        public DbSet<CarritoItem> CarritoItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Usuario
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.IdUsuario);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.IdUsuario)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Usuario>()
                .Property(u => u.NombreUsuario)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Correo)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Clave)
                .IsRequired()
                .HasMaxLength(100);

            // Configuración para Carrito
            modelBuilder.Entity<Carrito>()
                .HasKey(c => c.CarritoId);

            modelBuilder.Entity<Carrito>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Carrito)
                .HasForeignKey(ci => ci.CarritoId);

            // Configuración para CarritoItem
            modelBuilder.Entity<CarritoItem>()
                .HasKey(ci => new { ci.CarritoId, ci.ProductoId });

            modelBuilder.Entity<CarritoItem>()
                .HasOne(ci => ci.Producto)
                .WithMany()
                .HasForeignKey(ci => ci.ProductoId);

            // Configuración para ProductosPan
            modelBuilder.Entity<ProductosPan>()
                .HasKey(p => p.Id);
        }
    }
}
