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
        public DbSet<Anuncio> Anuncios { get; set; }
        public DbSet<Club> Clubes { get; set; }
        public DbSet<InvitacionClub> InvitacionClubes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configuramos que al borrar un Usuario NO se borren sus Reservas en cascada automáticamente
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Usuario)
                .WithMany() // Si no pusiste colección en Usuario, déjalo vacío así
                .HasForeignKey(r => r.idUsuario)
                .OnDelete(DeleteBehavior.Restrict); // O DeleteBehavior.NoAction

            // 2. Configuramos que al borrar una Pista NO se borren sus Reservas en cascada
            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Pista)
                .WithMany() // O .WithMany(p => p.Reservas) si tienes la colección
                .HasForeignKey(r => r.idPista)
                .OnDelete(DeleteBehavior.Restrict);

            // Si tienes Anuncios y te da el mismo error, haz lo mismo con ellos:
            modelBuilder.Entity<Anuncio>()
                .HasOne(a => a.usuario)
                .WithMany()
                .HasForeignKey(a => a.idUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }


}


