using Microsoft.EntityFrameworkCore;
using PadelApp.Datos;
using PadelApp.Modelos;
using PadelApp.Repositorios.IRepositorios;

namespace PadelApp.Repositorios
{
    public class RecuperarContraseñaRepositorio : IRecuperarContraseñaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public RecuperarContraseñaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GuardarCodigoAsync(RecuperacionPassword recuperacion)
        {
            // Opcional: Invalidar códigos anteriores del mismo email antes de guardar el nuevo
            var antiguos = _context.RecuperacionPasswords.Where(r => r.Email == recuperacion.Email && !r.Usado);
            foreach (var a in antiguos) a.Usado = true;

            await _context.RecuperacionPasswords.AddAsync(recuperacion);
            await _context.SaveChangesAsync();
        }

        public async Task<RecuperacionPassword?> ObtenerCodigoValidoAsync(string email, string codigo)
        {
            return await _context.RecuperacionPasswords
                .FirstOrDefaultAsync(r => r.Email == email &&
                                         r.Codigo == codigo &&
                                         !r.Usado &&
                                         r.FechaExpiracion > DateTime.Now);
        }

        public async Task MarcarComoUsadoAsync(int id)
        {
            var registro = await _context.RecuperacionPasswords.FindAsync(id);
            if (registro != null)
            {
                registro.Usado = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
