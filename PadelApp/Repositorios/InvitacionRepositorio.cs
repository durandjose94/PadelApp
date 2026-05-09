using Microsoft.EntityFrameworkCore;
using PadelApp.Datos;
using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;
using PadelApp.Repositorios.IRepositorios;
using PadelApp.Servicios.IServicios;

namespace PadelApp.Repositorios
{
    public class InvitacionRepositorio : IInvitacionRepositorio
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailServicio _emailServicio;

        public InvitacionRepositorio(ApplicationDbContext db, IEmailServicio emailServicio)
        {
            _db = db;
            _emailServicio = emailServicio;
        }

        public async Task<InvitacionClub> CrearInvitacionAsync(CrearInvitacionDto dto)
        {
            // 1. Validar si el usuario ya existe en el sistema
            bool usuarioExiste = await _db.Usuarios.AnyAsync(u => u.email == dto.Email && u.idClub == dto.IdClub);
            if (usuarioExiste)
            {
                throw new Exception("Este correo electrónico ya está registrado como socio.");
            }

            // 2. Validar si ya tiene una invitación pendiente que no ha caducado
            bool invitacionPendiente = await _db.InvitacionClubes.AnyAsync(i =>
                i.Email == dto.Email && !i.Usado && i.FechaExpiracion > DateTime.Now);

            if (invitacionPendiente)
            {
                throw new Exception("Ya existe una invitación pendiente para este correo.");
            }

            // 3. Generar código aleatorio de 6 dígitos
            string nuevoCodigo = new Random().Next(100000, 999999).ToString();

            // 4. Crear el objeto del modelo
            var invitacion = new InvitacionClub
            {
                Email = dto.Email,
                Codigo = nuevoCodigo,
                IdClub = dto.IdClub,
                IdUsuarioCreador = dto.IdUsuarioCreador,
                FechaCreacion = DateTime.Now,
                FechaExpiracion = DateTime.Now.AddDays(1), // Caduca en 1 día
                Usado = false
            };

            // 5. Guardar en BD
            await _db.InvitacionClubes.AddAsync(invitacion);
            await _db.SaveChangesAsync();

            // 6. Enviar Email
            string asunto = "Invitación a unirse al Club";
            string mensajeHtml = $@"
                <h1>¡Hola!</h1>
                <p>Has sido invitado a unirte a nuestro club en PadelApp.</p>
                <p>Tu código de registro es: <strong>{nuevoCodigo}</strong></p>
                <p>Este código caduca en 24 horas y solo es válido para este correo electrónico.</p>
                <br>
                <p>¡Te esperamos en la pista!</p>";

            await _emailServicio.EnviarEmailAsync(dto.Email, asunto, mensajeHtml);

            return invitacion;
        }

        public async Task<InvitacionClub> ValidarInvitacionAsync(string email, string codigo)
        {
            return await _db.InvitacionClubes.FirstOrDefaultAsync(i =>
                i.Email == email &&
                i.Codigo == codigo &&
                !i.Usado &&
                i.FechaExpiracion > DateTime.Now);
        }

        public async Task<bool> MarcarComoUsadaAsync(string codigo)
        {
            var invitacion = await _db.InvitacionClubes.FirstOrDefaultAsync(i => i.Codigo == codigo);
            if (invitacion == null) return false;

            invitacion.Usado = true;
            return await _db.SaveChangesAsync() >= 0;
        }

        /*public async Task<int?> ObtenerClubPorInvitacionValidaAsync(string email, string codigo)
        {
            var invitacion = await _db.InvitacionClubes.FirstOrDefaultAsync(i =>
                i.Email == email &&
                i.Codigo == codigo &&
                !i.Usado &&
                i.FechaExpiracion > DateTime.Now);

            // Si existe y es válida, devolvemos el IdClub, si no, null
            return invitacion?.IdClub;
        }*/
    }
}
