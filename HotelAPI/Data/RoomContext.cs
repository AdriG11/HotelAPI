using ClasesModelos;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Data
{
    public class RoomContext : DbContext
    {
        public RoomContext(DbContextOptions<RoomContext> options) :
            base(options)
        {

        }
        public DbSet<Habitaciones> Habitaciones {get; set;}
        public DbSet<Reservas> Reservation { get; set;}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar clave primaria para Datos
            modelBuilder.Entity<Habitaciones>().HasKey(d => d.RoomId);

            // Configurar clave primaria para RegistroNomina
            modelBuilder.Entity<Reservas>().HasKey(r => r.RoomId);

            modelBuilder.Entity<Reservas>()
           .HasOne(r => r.Habitaciones)
           .WithMany(d => d.Reservas)
           .HasForeignKey(r => r.RoomId)
           .IsRequired();
        }
    }


}

