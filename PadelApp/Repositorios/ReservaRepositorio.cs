using Microsoft.EntityFrameworkCore;
using PadelApp.Datos;
using PadelApp.Modelos;
using PadelApp.Repositorios.IRepositorios;
using System.Collections.Generic;

namespace PadelApp.Repositorios
{
    public class ReservaRepositorio : IReservaRepositorio
    {
        private readonly ApplicationDbContext _db;
        public ReservaRepositorio(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Reserva>> GetReservasAsync(int idClub)
        {
            return await _db.Reservas
                .Include(r => r.Pista).ThenInclude(p => p.Sede)
                .Include(r => r.Usuario).Where(r => r.Usuario.idClub == idClub)
                .OrderByDescending(r => r.fecha_reserva)
                .ToListAsync();
        }

        public async Task<Reserva> GetReservaAsync(int idReserva, int idClub)
        {
            return await _db.Reservas
                .Include(r => r.Pista).ThenInclude(p => p.Sede)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.idReserva == idReserva && r.Usuario.idClub == idClub);
        }

        public async Task<IEnumerable<Reserva>> GetReservasPorUsuarioAsync(int idUsuario, int idClub)
        {
            return await _db.Reservas
                .Include(r => r.Pista).ThenInclude(p => p.Sede)
                .Where(r => r.idUsuario == idUsuario && r.Usuario.idClub == idClub)
                .OrderByDescending(r => r.fecha_reserva)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> GetReservasPorSedeAsync(int idSede, int idClub)
        {
            return await _db.Reservas
                    // Cargamos la Pista y su Sede relacionada para tener los datos completos
                    .Include(r => r.Pista)
                        .ThenInclude(p => p.Sede)
                        .Include(r => r.Usuario)
                    // Filtramos: "Dame las reservas donde el idSede de la Pista sea igual al parámetro"
                    .Where(r => r.Pista.idSede == idSede && r.Usuario.idClub == idClub)
                    .OrderByDescending(r => r.fecha_reserva)
                    //.ThenByDescending(r => r.hora_inicio) // Opcional: ordenar también por hora
                    .ToListAsync();
        }

        public async Task<bool> CrearReservaAsync(Reserva reserva)
        {
            reserva.fecha_registro = DateTime.Now;
            reserva.estado = EstadoReserva.Pagada; //Se crea al pagar
            await _db.Reservas.AddAsync(reserva);
            return await GuardarAsync();
        }

        public async Task<bool> ActualizarReservaAsync(Reserva reserva)
        {
            reserva.fecha_actualizacion = DateTime.Now;
            _db.Reservas.Update(reserva);
            return await GuardarAsync();
        }

        public async Task<bool> CancelarReservaAsync(int idReserva)
        {
            var reserva = await _db.Reservas.FindAsync(idReserva);
            if (reserva == null) return false;

            // Si ya está cancelada, no hacemos nada pero devolvemos true o manejamos según necesites
            if (reserva.estado == EstadoReserva.Cancelada) return true;

            reserva.estado = EstadoReserva.Cancelada;
            reserva.fecha_actualizacion = DateTime.Now;

            _db.Reservas.Update(reserva);
            return await GuardarAsync();
        }

        public async Task<bool> ExisteReservaAsync(int idReserva, int idClub)
        {
            return await _db.Reservas.AnyAsync(r => r.idReserva == idReserva && r.Usuario.idClub == idClub);
        }

        public async Task<IEnumerable<Reserva>> GetReservasPorPistaYFechaAsync(int idPista, DateOnly fecha, int idClub)
        {
            return await _db.Reservas
                .Where(r => r.idPista == idPista &&
                            r.fecha_reserva == fecha &&
                            r.estado != EstadoReserva.Cancelada &&
                            r.Usuario.idClub == idClub)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> GetReservasConFiltroAsync(string filtro, int idSede, int idClub)
        {
            var query = _db.Reservas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.ToLower().Trim();
                query = query.Include(r => r.Usuario).Where(u =>
                    u.Usuario.idClub == idClub &&
                    u.Pista.idSede == idSede &&
                    (u.Usuario.nombre.ToLower().Contains(f) ||
                    u.Usuario.apellidos.ToLower().Contains(f) ||
                    (u.Usuario.nombre + " " + u.Usuario.apellidos).ToLower().Contains(f))
                );
            }

            return await query.OrderBy(u => u.Usuario.nombre).ToListAsync();
        }

        public async Task<bool> GuardarAsync()
        {
            return await _db.SaveChangesAsync() >= 0;
        }
    }
}
