using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;
using PadelApp.Repositorios.IRepositorios;
using PadelApp.Servicios.IServicios;

namespace PadelApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaRepositorio _reservaRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPistaRepositorio _pistaRepositorio;
        private readonly IComprobanteServicio _comprobanteServicio;
        private readonly IMapper _mapper;

        public ReservaController(
            IReservaRepositorio reservaRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IPistaRepositorio pistaRepositorio,
            IComprobanteServicio comprobanteServicio,
            IMapper mapper)
        {
            _reservaRepositorio = reservaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _pistaRepositorio = pistaRepositorio;
            _comprobanteServicio = comprobanteServicio;
            _mapper = mapper;
        }

        [HttpGet("{idReserva:int}", Name = "GetReserva")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReserva(int idReserva)
        {
            var reserva = await _reservaRepositorio.GetReservaAsync(idReserva);
            if (reserva == null) return NotFound();

            var reservaDto = _mapper.Map<ReservaDto>(reserva);
            return Ok(reservaDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetReservas()
        {
            var listaReservas = await _reservaRepositorio.GetReservasAsync();
            var listaReservasDto = _mapper.Map<IEnumerable<ReservaDto>>(listaReservas);
            return Ok(listaReservasDto);
        }

        [HttpGet("Usuario/{idUsuario:int}")]
        public async Task<IActionResult> GetReservasPorUsuario(int idUsuario)
        {
            var listaReservas = await _reservaRepositorio.GetReservasPorUsuarioAsync(idUsuario);
            /*if (listaReservas == null || !listaReservas.Any())
                return NotFound("No se encontraron reservas para este usuario.");*/

            var listaReservasDto = _mapper.Map<IEnumerable<ReservaDto>>(listaReservas);
            return Ok(listaReservasDto);
        }

        [HttpGet("Sede/{idSede:int}")]
        public async Task<IActionResult> GetReservasPorSede(int idSede)
        {
            var listaReservas = await _reservaRepositorio.GetReservasPorSedeAsync(idSede);
            // Si listaReservas es null, lo convertimos a una lista vacía para que el Map no falle
            if (listaReservas == null) listaReservas = new List<Reserva>();

            // Siempre devolvemos Ok. Si no hay nada, el JSON será []
            var listaReservasDto = _mapper.Map<IEnumerable<ReservaDto>>(listaReservas);
            return Ok(listaReservasDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmarPagarReserva([FromBody] CrearReservaDto crearReservaDto)
        {
            if (crearReservaDto == null || !ModelState.IsValid) return BadRequest(ModelState);

            // 1. Validación de lógica de negocio (Horas exactas)
            if (crearReservaDto.hora_inicio.Minute != 0 || crearReservaDto.hora_fin.Hour != crearReservaDto.hora_inicio.Hour + 1)
                return BadRequest("Solo se permiten reservas de bloques de una hora exacta.");

            // --- NUEVA VALIDACIÓN: ESTADO DE LA PISTA ---
            var pista = await _pistaRepositorio.GetPistaAsync(crearReservaDto.idPista);

            if (pista == null)
                return NotFound("La pista especificada no existe.");

            // Comparación directa con el Enum
            if (pista.estado != EstadoPista.Disponible)
            {
                return BadRequest("No se puede reservar: La pista no se encuentra disponible actualmente.");
            }

            // Validamos si está activo o no.
            if (!pista.activo)
            {
                return BadRequest("La pista seleccionada ha sido dada de baja.");
            }
            // ----------------------------------------------------------------

            // 2. Validación de disponibilidad real
            var reservasExistentes = await _reservaRepositorio.GetReservasPorPistaYFechaAsync(crearReservaDto.idPista, crearReservaDto.fecha_reserva);

            bool ocupada = reservasExistentes.Any(r => r.hora_inicio == crearReservaDto.hora_inicio);

            if (ocupada)
                return StatusCode(StatusCodes.Status409Conflict, "La pista ya está reservada para esa hora.");

            // 3. Mapeo y creación
            var reserva = _mapper.Map<Reserva>(crearReservaDto);

            if (!await _reservaRepositorio.CrearReservaAsync(reserva))
            {
                ModelState.AddModelError("", $"Algo salió mal al guardar la reserva");
                return StatusCode(500, ModelState);
            }

            // --- PROCESO DE COMPROBANTE Y ENVÍO (NUEVO) ---
            try
            {
                // Buscamos los datos del usuario para el PDF y el Email
                var usuario = await _usuarioRepositorio.GetUsuarioAsync(reserva.idUsuario);
                //Buscamos los datos de la reserva completa (con pista y sede) para el PDF                

                if (usuario != null && pista != null)
                {
                    // Generamos el PDF en memoria (byte[])
                    byte[] pdfArchivo = _comprobanteServicio.GenerarPdfReserva(reserva, usuario, pista);

                    // Enviamos el email con el PDF adjunto
                    // Nota: No usamos 'await' aquí si no queremos que el usuario espere a que el mail se envíe para ver el éxito,
                    // pero lo más seguro es usar await para confirmar que el proceso terminó.
                    await _comprobanteServicio.EnviarEmailConComprobante(usuario.email, pdfArchivo, reserva);
                }
            }
            catch (Exception ex)
            {
                // Importante: Si el correo falla, la reserva YA está hecha. 
                // Solo logueamos el error para no asustar al usuario con un error 500.
                Console.WriteLine($"Reserva OK, pero falló el envío del comprobante: {ex.Message}");
            }

            return CreatedAtRoute("GetReserva", new { idReserva = reserva.idReserva }, reserva);
        }

        [HttpPatch("Cancelar/{idReserva:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelarReserva(int idReserva)
        {
            // 1. Obtener la reserva completa
            var reserva = await _reservaRepositorio.GetReservaAsync(idReserva);

            if (reserva == null)
                return NotFound();

            // 2. Lógica de tiempo: Combinar Fecha y Hora de inicio
            // Asumiendo que fecha_reserva es DateOnly y hora_inicio es string "08:00"
            // Si reserva.hora_inicio YA ES TimeOnly, úsala directamente:
            var fechaHoraReserva = reserva.fecha_reserva.ToDateTime(reserva.hora_inicio);
            var ahora = DateTime.Now;

            // 3. Validación: ¿Ya pasó la hora o falta menos de, por ejemplo, 2 horas?
            if (fechaHoraReserva <= ahora)
            {
                return BadRequest("No se puede cancelar una reserva que ya ha comenzado o ha finalizado.");
            }

            /* Opcional: Política de cancelación (ej. 2 horas de antelación)
            if ((fechaHoraReserva - ahora).TotalHours < 2)
            {
                return BadRequest("Las reservas solo pueden cancelarse con un mínimo de 2 horas de antelación.");
            }
            */

            // 4. Proceder con la cancelación
            if (!await _reservaRepositorio.CancelarReservaAsync(idReserva))
            {
                return StatusCode(500, "Error al intentar cancelar la reserva.");
            }

            return NoContent();
        }

        [Authorize]
        [HttpGet("disponibilidad/{idPista}/{fecha}")]
        public async Task<ActionResult<DisponibilidadDto>> GetDisponibilidad(int idPista, DateOnly fecha)
        {
            var reservas = await _reservaRepositorio.GetReservasPorPistaYFechaAsync(idPista, fecha);

            // Datos para validar si la hora ya pasó
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            var ahora = TimeOnly.FromDateTime(DateTime.Now);

            var disponibilidad = new DisponibilidadDto
            {
                IdPista = idPista,
                Fecha = fecha,
                Horarios = new List<BloqueHorarioDto>()
            };

            for (int h = 8; h <= 22; h++)
            {
                var horaBloque = new TimeOnly(h, 0);
                bool estaOcupada = reservas.Any(r => r.hora_inicio == horaBloque);

                // Lógica de "Hora Pasada"
                bool esPasada = (fecha < hoy) || (fecha == hoy && horaBloque < ahora);

                disponibilidad.Horarios.Add(new BloqueHorarioDto
                {
                    Hora = horaBloque.ToString("HH:mm"),
                    // Solo está disponible si NO está ocupada Y NO ha pasado ya
                    Disponible = !estaOcupada && !esPasada,
                    // Opcional: podrías añadir una propiedad "EsPasada" si quieres un color distinto al rojo
                    EsPasada = esPasada
                });
            }

            return Ok(disponibilidad);
        }

        [Authorize]
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReservaDto>>> GetReservasFiltradas([FromQuery] string filtro, [FromQuery] int idSede)
        {
            var reservas = await _reservaRepositorio.GetReservasConFiltroAsync(filtro, idSede);

            var reservasDto = _mapper.Map<IEnumerable<ReservaDto>>(reservas);

            return Ok(reservasDto);
        }
    }
}
