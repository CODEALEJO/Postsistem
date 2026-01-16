using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Postsistem.Models;


namespace Postsistem.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly UtcDateInterceptor _utcInterceptor = new();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Local> Locales { get; set; }
        public DbSet<UsuarioLocal> UsuarioLocales { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<ProductoVenta> ProductosVenta { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<MetodoPago> MetodoPagos { get; set; }
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<Salida> Salidas { get; set; }
        public DbSet<MovimientoCaja> MovimientoCajas { get; set; }
        public DbSet<ManejoClientes> ManejoClientes { get; set; }
        public DbSet<AbonoCliente> AbonoClientes { get; set; }
        public DbSet<DevolucionGarantia> DevolucionGarantias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductoVenta>()
                .Property(p => p.PrecioFinal)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<Venta>()
                .Property(v => v.DescuentoTotal)
                .HasColumnType("numeric(18,2)");

            modelBuilder.Entity<Local>().HasData(
   new Local { Id = 1, Nombre = "Tris de Amor" },
   new Local { Id = 2, Nombre = "Los Brothers" }
);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_utcInterceptor);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
