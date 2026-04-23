using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PadelApp.Datos;
using PadelApp.Helpers;
using PadelApp.Modelos;
using PadelApp.Repositorios.IRepositorios;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PadelApp.Repositorios
{
    public enum ResultadoBorradoUsuario
    {
        Exito = 0,
        TieneReservasPagadasActivas = 2,
        ErrorServidor = 3
    }
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db;
        private readonly string claveSecreta;

        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
        }

        public async Task<ResultadoBorradoUsuario> EliminarUsuarioAsync(Usuario usuario)
        {
            var ahora = DateTime.Now;

            DateOnly fechaHoy = DateOnly.FromDateTime(ahora);
            TimeOnly horaActual = TimeOnly.FromDateTime(ahora);

            // Buscamos reservas pagadas que:
            // 1. Sean de una fecha futura.
            // 2. O sean de hoy, pero la hora de fin aún no haya pasado.
            bool tieneReservaActiva = await _db.Reservas
                .AnyAsync(r => r.idUsuario == usuario.idUsuario &&
                               r.estado == EstadoReserva.Pagada &&
                               (r.fecha_reserva > fechaHoy ||
                               (r.fecha_reserva == fechaHoy && r.hora_fin > horaActual)));

            bool tieneReservas = await _db.Reservas.AnyAsync(r => r.Usuario.idUsuario == usuario.idUsuario);

            if (tieneReservas)
            {
                usuario.activo = false;
                usuario.fecha_actualizacion = DateTime.Now;
                _db.Usuarios.Update(usuario);
            }
            else
            {
                _db.Usuarios.Remove(usuario);
            }

            return await GuardarAsync() ? ResultadoBorradoUsuario.Exito : ResultadoBorradoUsuario.ErrorServidor;
        }

        public async Task<bool> UsuarioUnicoAsync(string email)
        {
            // Usamos AnyAsync porque es más rápido que FirstOrDefault para solo verificar existencia
            return !await _db.Usuarios.AnyAsync(u => u.email == email && u.activo);
        }

        public async Task<bool> ExisteUsuarioAsync(int idUsuario)
        {
            return await _db.Usuarios.AnyAsync(s => s.idUsuario == idUsuario);
        }

        public async Task<Usuario> GetUsuarioAsync(int idUsuario)
        {
            return await _db.Usuarios.FirstOrDefaultAsync(u => u.idUsuario == idUsuario);
        }

        public async Task<Usuario> GetUsuarioAsync(string email)
        {
            return await _db.Usuarios.FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosAsync()
        {
            return await _db.Usuarios
                .Where(u => u.activo == true && u.idRol == 2)
                .OrderBy(u => u.nombre)
                .ToListAsync();
        }

        public async Task<bool> ModificarUsuarioAsync(Usuario usuario)
        {
            usuario.fecha_actualizacion = DateTime.Now;
            _db.Usuarios.Update(usuario);
            return await GuardarAsync();
        }

        public async Task<Usuario> RegistrarUsuarioAsync(Usuario usuario)
        {
            usuario.password = PasswordEncripted.EncriptarPassword(usuario.password);
            usuario.activo = true;
            usuario.fecha_registro = DateTime.Now;
            usuario.idRol = 2;
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            return usuario;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosConFiltroAsync(string filtro)
        {
            var query = _db.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                var f = filtro.ToLower().Trim();
                query = query.Where(u =>
                u.idRol == 2 &&
                u.activo == true && (
                    u.nombre.ToLower().Contains(f) ||
                    u.apellidos.ToLower().Contains(f) ||
                    (u.nombre + " " + u.apellidos).ToLower().Contains(f)
                ));
            }

            return await query.OrderBy(u => u.nombre).ToListAsync();
        }

        public async Task<bool> ActualizarPasswordAsync(string email, string nuevaPassword)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.email == email);

            if (usuario == null) return false;

            // Aplicamos tu helper de encriptación
            usuario.password = PasswordEncripted.EncriptarPassword(nuevaPassword);
            usuario.fecha_actualizacion = DateTime.Now;

            // Entity Framework es inteligente: al modificar estas propiedades 
            // en un objeto rastreado, solo actualizará esos campos en el SQL final.
            _db.Usuarios.Update(usuario);
            return await GuardarAsync();
        }

        public async Task<bool> GuardarAsync()
        {
            // Devuelve true si se guardó al menos un cambio (o 0 si no hubo cambios pero no hubo error)
            return await _db.SaveChangesAsync() >= 0;
        }
    }
}
