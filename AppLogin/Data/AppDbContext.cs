using Microsoft.EntityFrameworkCore;
using AppLogin.Models;

namespace AppLogin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //definiendo las caracteristicas deseadas de mi tabla
            modelBuilder.Entity<User>(tb =>
            {
                tb.HasKey(col => col.Id);
                tb.Property(col => col.Id).UseIdentityColumn().ValueGeneratedOnAdd();

                tb.Property(col => col.Name).HasMaxLength(50);
                tb.Property(col => col.LastName).HasMaxLength(50);
                tb.Property(col => col.Email).HasMaxLength(50);
                tb.Property(col => col.Password).HasMaxLength(50);
            });

            modelBuilder.Entity<User>().ToTable("User");
        }

    }
}
