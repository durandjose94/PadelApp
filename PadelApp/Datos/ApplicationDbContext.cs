using Microsoft.EntityFrameworkCore;
using PadelApp.Modelos;

namespace PadelApp.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Pista> Pistas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<RecuperacionPassword> RecuperacionPasswords { get; set; }
    }
}
