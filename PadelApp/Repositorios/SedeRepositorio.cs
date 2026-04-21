using Microsoft.EntityFrameworkCore;
using PadelApp.Datos;
using PadelApp.Modelos;
using PadelApp.Repositorios.IRepositorios;

namespace PadelApp.Repositorios
{
    public enum ResultadoBorradoSede
    {
        Exito = 0,
        TienePistasActivas = 1,
        TieneReservasPagadas = 2,
        ErrorServidor = 3
    }

    public class SedeRepositorio : ISedeRepositorio
    {
        private readonly ApplicationDbContext _db;

        public SedeRepositorio(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ActualizarSedeAsync(Sede sede)
        {
            sede.fecha_actualizacion = DateTime.Now;
            _db.Sedes.Update(sede);
            return await GuardarAsync();
        }

        public async Task<bool> CrearSedeAsync(Sede sede)
        {
            sede.activo = true;
            sede.fecha_registro = DateTime.Now;
            await _db.Sedes.AddAsync(sede);
            return await GuardarAsync();
        }

        public async Task<ResultadoBorradoSede> EliminarSedeAsync(Sede sede)
        {
            if (await _db.Pistas.AnyAsync(r => r.idSede == sede.idSede && r.activo))
                return ResultadoBorradoSede.TienePistasActivas;

            if (await _db.Reservas.AnyAsync(r => r.Pista.idSede == sede.idSede && r.estado == EstadoReserva.Pagada))
                return ResultadoBorradoSede.TieneReservasPagadas;

            bool tienePistasInactivas = await _db.Pistas.AnyAsync(r => r.idSede == sede.idSede && !r.activo);

            if (tienePistasInactivas)
            {
                sede.activo = false;
                sede.fecha_actualizacion = DateTime.Now;
                _db.Sedes.Update(sede);
            }
            else
            {
                _db.Sedes.Remove(sede);
            }

            return await GuardarAsync() ? ResultadoBorradoSede.Exito : ResultadoBorradoSede.ErrorServidor;
        }

        public async Task<bool> ExisteSedeAsync(int idSede)
        {
            return await _db.Sedes.AnyAsync(s => s.idSede == idSede);
        }

        public async Task<bool> ExisteSedeAsync(string nombreSede)
        {
            string nombreNormalizado = nombreSede.ToLower().Trim();
            return await _db.Sedes.AnyAsync(s =>
                s.nombreSede.ToLower().Trim() == nombreNormalizado && s.activo);
        }

        public async Task<Sede> GetSedeAsync(int idSede)
        {
            return await _db.Sedes.FirstOrDefaultAsync(s => s.idSede == idSede);
        }

        public async Task<IEnumerable<Sede>> GetSedesAsync()
        {
            return await _db.Sedes
                .Where(s => s.activo == true)
                .OrderBy(s => s.nombreSede)
                .ToListAsync();
        }

        public async Task<bool> GuardarAsync()
        {
            // SaveChangesAsync devuelve el número de filas afectadas
            return await _db.SaveChangesAsync() >= 0;
        }
    }
}