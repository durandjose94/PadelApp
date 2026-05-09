using Microsoft.EntityFrameworkCore;
using PadelApp.Datos;
using PadelApp.Modelos;
using PadelApp.Repositorios.IRepositorios;

namespace PadelApp.Repositorios
{
    public class ClubRepositorio : IClubRepositorio
    {
        private readonly ApplicationDbContext _db;

        public ClubRepositorio(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Club>> GetClubesPorEmailAsync(string email)
        {
            return await _db.Usuarios
                .Where(u => u.email == email && u.activo == true)
                .Select(u => u.Club) 
                .Where(c => c.activo == true)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Club> GetClubByIdAsync(int idClub)
        {
            return await _db.Clubes.FirstOrDefaultAsync(c => c.idClub == idClub && c.activo == true);
        }

        public async Task<IEnumerable<Club>> GetClubesAsync()
        {
            return await _db.Clubes.Where(c => c.activo == true).ToListAsync();
        }

        public async Task<bool> ActualizarClubAsync(Club club)
        {
            club.fecha_actualizacion = DateTime.Now;
            _db.Clubes.Update(club);
            return await GuardarAsync();
        }

        public async Task<bool> GuardarAsync()
        {
            return await _db.SaveChangesAsync() >= 0;
        }
    }
}
