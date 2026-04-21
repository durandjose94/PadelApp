using Microsoft.EntityFrameworkCore;
using PadelApp.Datos;
using PadelApp.Modelos;
using PadelApp.Repositorios.IRepositorios;

namespace PadelApp.Repositorios
{
    public enum ResultadoBorradoPista
    {
        Exito = 0,
        TieneReservasPagadas = 2,
        ErrorServidor = 3
    }
    public class PistaRepositorio : IPistaRepositorio
    {
        private readonly ApplicationDbContext _db;

        public PistaRepositorio(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> ActualizarPistaAsync(Pista pista)
        {
            try
            {
                pista.fecha_actualizacion = DateTime.Now;
                _db.Pistas.Update(pista);
                return await GuardarAsync();
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> CrearPistaAsync(Pista pista)
        {
            try
            {
                pista.fecha_registro = DateTime.Now;
                await _db.Pistas.AddAsync(pista);
                return await GuardarAsync();
            }
            catch
            {
                return false;
            }
        }
        public async Task<ResultadoBorradoPista> EliminarPistaAsync(Pista pista)
        {
            if (await _db.Reservas.AnyAsync(r => r.Pista.idPista == pista.idPista && r.estado == EstadoReserva.Pagada))
                return ResultadoBorradoPista.TieneReservasPagadas;

            bool tieneReservas = await _db.Reservas.AnyAsync(r => r.Pista.idPista == pista.idPista);

            if (tieneReservas)
            {
                pista.activo = false;
                pista.fecha_actualizacion = DateTime.Now;
                _db.Pistas.Update(pista);
            }
            else
            {
                _db.Pistas.Remove(pista);
            }

            return await GuardarAsync() ? ResultadoBorradoPista.Exito : ResultadoBorradoPista.ErrorServidor;
        }
        public async Task<bool> ExistePistaAsync(int idPista)
        {
            return await _db.Pistas.AnyAsync(s => s.idPista == idPista);
        }
        public async Task<bool> ExistePistaAsync(string nombrePista)
        {
            return await _db.Pistas.AnyAsync(s => s.nombrePista.ToLower().Trim() == nombrePista.ToLower().Trim() && s.activo);
        }
        public async Task<Pista> GetPistaAsync(int idPista)
        {
            return await _db.Pistas
                .Include(p => p.Sede)
                .FirstOrDefaultAsync(p => p.idPista == idPista);
        }
        public async Task<ICollection<Pista>> GetPistasAsync()
        {
            return await _db.Pistas.Where(p => p.activo == true)
                .Include(p => p.Sede) // Incluimos la sede para que el DTO tenga el nombre
                .OrderBy(s => s.nombrePista)
                .ToListAsync();
        }
        public async Task<ICollection<Pista>> GetPistasPorSedeAsync(int idSede)
        {
            return await _db.Pistas
                .Where(p => p.idSede == idSede && p.activo == true)
                .Include(p => p.Sede) // Incluimos la sede para que el DTO tenga el nombre
                .ToListAsync();
        }
        public async Task<bool> GuardarAsync()
        {
            try
            {
                // Si SaveChangesAsync devuelve 0 o más, significa que la operación fue exitosa
                return await _db.SaveChangesAsync() >= 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
