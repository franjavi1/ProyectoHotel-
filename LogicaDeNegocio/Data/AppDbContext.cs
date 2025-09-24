using HotelApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LogicaDeNegocio.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Huesped> Clientes { get; set; }

        // Configuración adicional
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Especificar el tipo decimal para evitar truncamiento
            modelBuilder.Entity<Habitacion>()
                .Property(h => h.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Reserva>()
                .Property(r => r.PrecioTotal)
                .HasColumnType("decimal(18,2)");
        }


    }
}
