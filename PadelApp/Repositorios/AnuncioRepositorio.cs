using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        public async Task<IEnumerable<Anuncio>> GetAnunciosActivosAsync(int idClub)
        {
            return await _context.Anuncios
                .Include(a => a.usuario)
                .Where(a => a.fechaExpiracion >= DateTime.Now && a.usuario.idClub == idClub)
                .OrderByDescending(a => a.fecha_registro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Anuncio>> GetAnunciosByUsuarioAsync(int idUsuario, int idClub)
        {
            var fechaCorte = DateTime.Now.AddDays(-10);

            return await _context.Anuncios
                .Where(a => a.idUsuario == idUsuario && a.usuario.idClub == idClub &&
                   (a.fechaExpiracion >= DateTime.Now || a.fechaExpiracion >= fechaCorte))
                .OrderByDescending(a => a.fechaExpiracion)
                .OrderByDescending(a => a.fecha_registro)
                .ToListAsync();
        }

        public async Task<Anuncio> GetAnuncioByIdAsync(int idAnuncio, int idClub)
        {
            return await _context.Anuncios
                .Include(a => a.usuario)
                .FirstOrDefaultAsync(a => a.idAnuncio == idAnuncio && a.usuario.idClub == idClub);
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

        public async Task<int> CountAnunciosActivosUsuarioAsync(int idUsuario, TipoAnuncio tipo, int idClub)
        {
            return await _context.Anuncios
                .CountAsync(a => a.idUsuario == idUsuario &&
                                 a.usuario.idClub == idClub &&
                                 a.tipoAnuncio == tipo &&
                                 a.fechaExpiracion >= DateTime.Now);
        }
    }
}
