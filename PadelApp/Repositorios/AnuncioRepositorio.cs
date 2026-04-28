using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PadelApp.Datos;
using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;
using PadelApp.Repositorios.IRepositorios;

namespace PadelApp.Repositorios
{
    public class AnuncioRepositorio : IAnuncioRepositorio
    {
        private readonly ApplicationDbContext _context;

        public AnuncioRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Anuncio>> GetAnunciosActivosAsync()
        {
            return await _context.Anuncios
                .Include(a => a.usuario)
                .Where(a => a.fechaExpiracion >= DateTime.Now)
                .OrderByDescending(a => a.fecha_registro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Anuncio>> GetAnunciosByUsuarioAsync(int idUsuario)
        {
            return await _context.Anuncios
                .Where(a => a.idUsuario == idUsuario)
                .OrderByDescending(a => a.fecha_registro)
                .ToListAsync();
        }

        public async Task<Anuncio> GetAnuncioByIdAsync(int idAnuncio)
        {
            return await _context.Anuncios
                .Include(a => a.usuario)
                .FirstOrDefaultAsync(a => a.idAnuncio == idAnuncio);
        }

        public async Task<Anuncio> CrearAnuncioAsync(Anuncio anuncio)
        {
            anuncio.fecha_registro = DateTime.Now;

            // Lógica de Caducidad
            if (anuncio.tipoAnuncio == TipoAnuncio.Partido && anuncio.fechaEvento.HasValue)
            {
                anuncio.fechaExpiracion = anuncio.fechaEvento.Value.Date.AddDays(1).AddSeconds(-1);
            }
            else
            {
                anuncio.fechaExpiracion = DateTime.Now.AddDays(15);
            }

            await _context.Anuncios.AddAsync(anuncio);
            await _context.SaveChangesAsync();
            return anuncio;
        }

        public async Task<bool> ActualizarAnuncioAsync(Anuncio anuncio)
        {
            anuncio.fecha_actualizacion = DateTime.Now;

            // Recalcular expiración si es un partido
            if (anuncio.tipoAnuncio == TipoAnuncio.Partido && anuncio.fechaEvento.HasValue)
            {
                anuncio.fechaExpiracion = anuncio.fechaEvento.Value.Date.AddDays(1).AddSeconds(-1);
            }

            _context.Anuncios.Update(anuncio);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> EliminarAnuncioAsync(int idAnuncio)
        {
            var anuncio = await _context.Anuncios.FindAsync(idAnuncio);
            if (anuncio == null) return false;

            _context.Anuncios.Remove(anuncio);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> CountAnunciosActivosUsuarioAsync(int idUsuario, TipoAnuncio tipo)
        {
            return await _context.Anuncios
                .CountAsync(a => a.idUsuario == idUsuario &&
                                 a.tipoAnuncio == tipo &&
                                 a.fechaExpiracion >= DateTime.Now);
        }
    }
}
