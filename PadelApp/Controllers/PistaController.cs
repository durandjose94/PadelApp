using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;
using PadelApp.Repositorios;
using PadelApp.Repositorios.IRepositorios;

namespace PadelApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PistaController : ControllerBase
    {
        private readonly IPistaRepositorio _pistaRepositorio;
        private readonly IMapper _mapper;

        public PistaController(IPistaRepositorio pistaRepositorio, IMapper mapper)
        {
            _pistaRepositorio = pistaRepositorio;
            _mapper = mapper;
        }

        [HttpGet("{idPista:int}", Name = "GetPista")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPista(int idPista)
        {
            var pista = await _pistaRepositorio.GetPistaAsync(idPista);

            if (pista == null)
            {
                return NotFound();
            }

            var pistaDto = _mapper.Map<PistaDto>(pista);

            return Ok(pistaDto);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetPistas()
        {
            // 1. Obtenemos la colección de forma asíncrona
            // Recuerda que en el repositorio añadimos .Include(p => p.Sede)
            var listaPistas = await _pistaRepositorio.GetPistasAsync();

            // 2. Mapeamos a la lista de DTOs
            // El Mapper se encarga de convertir la ICollection en IEnumerable/List para el DTO
            var listaPistasDto = _mapper.Map<IEnumerable<PistaDto>>(listaPistas);

            // 3. Devolvemos un 200 OK con la lista (vacía o llena)
            return Ok(listaPistasDto);
        }

        [HttpGet("Sede/{idSede:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPistasPorSede(int idSede)
        {
            // 1. Llamada asíncrona al repositorio
            var listaPistas = await _pistaRepositorio.GetPistasPorSedeAsync(idSede);

            // Nota: Es mejor devolver una lista vacía [] con un 200 OK si no hay pistas, 
            // en lugar de un 404, para que el Front-end pueda manejar el estado "Sin resultados" fácilmente.
            if (listaPistas == null)
            {
                return NotFound("No se pudo procesar la solicitud para esta sede.");
            }

            // 2. Mapeo a DTO
            // AutoMapper maneja automáticamente colecciones (IEnumerable, ICollection, List)
            var listaPistasDto = _mapper.Map<IEnumerable<PistaDto>>(listaPistas);

            return Ok(listaPistasDto);
        }

        [HttpPut("{idPista:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarPista(int idPista, [FromBody] ModificarPistaDto modificarPistaDto)
        {
            if (modificarPistaDto == null) return BadRequest("Los datos de entrada no pueden ser nulos.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var pista = await _pistaRepositorio.GetPistaAsync(idPista);

            if (pista == null)
            {
                return NotFound($"No se encontró la pista con id : {idPista}");
            }

            if(!pista.activo) return BadRequest("No se puede actualizar una pista inactiva.");

            _mapper.Map(modificarPistaDto, pista);

            if (!await _pistaRepositorio.ActualizarPistaAsync(pista))
            {
                return BadRequest(new { message = "No se pudieron aplicar los cambios en el modelo." });
            }

            if (!await _pistaRepositorio.GuardarAsync())
            {
                return StatusCode(500, $"Error al persistir los cambios en la base de datos para la pista {idPista}");
            }

            return Ok(pista);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearPista([FromBody] CrearPistaDto crearPistaDto)
        {
            if (crearPistaDto == null) return BadRequest("Los datos de la pista son requeridos.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _pistaRepositorio.ExistePistaAsync(crearPistaDto.nombrePista))
            {
                return Conflict("Ya existe una pista con ese nombre.");
            }

            var pista = _mapper.Map<Pista>(crearPistaDto);

            if (!await _pistaRepositorio.CrearPistaAsync(pista))
            {
                return StatusCode(500, "Ocurrió un error inesperado al intentar registrar la pista en el servidor.");
            }

            return CreatedAtRoute("GetPista", new { idPista = pista.idPista }, pista);
        }

        [HttpDelete("{idPista:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BorrarPista(int idPista)
        {
            var pista = await _pistaRepositorio.GetPistaAsync(idPista);
            if (pista == null) return NotFound();

            var result = await _pistaRepositorio.EliminarPistaAsync(pista);

            return result switch
            {
                ResultadoBorradoPista.TieneReservasPagadas => BadRequest("No se puede eliminar la pista porque tiene reservas pagadas."),
                ResultadoBorradoPista.ErrorServidor => StatusCode(500, "Error interno al procesar el borrado."),
                ResultadoBorradoPista.Exito => NoContent(),
                _ => StatusCode(500)
            };
        }

    }
}
