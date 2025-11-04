using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POSTSISTEM.Models;
using Microsoft.AspNetCore.Identity;
using Postsistem.Models;

namespace POSTSISTEM.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly UtcDateInterceptor _utcInterceptor = new();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

         public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Necesario para Identity
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_utcInterceptor);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
