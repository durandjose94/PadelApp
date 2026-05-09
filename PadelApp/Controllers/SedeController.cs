using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class SedeController : PadelControllerBase
    {
        private readonly ISedeRepositorio _sedeRepositorio;
        private readonly IMapper _mapper;

        public SedeController(ISedeRepositorio sedeRepositorio, IMapper mapper)
        {
            _sedeRepositorio = sedeRepositorio;
            _mapper = mapper;
        }

        [HttpGet("{idSede:int}", Name = "GetSede")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSede(int idSede)
        {
            int idClub = UsuarioClubId;
            if (idClub <= 0) return Unauthorized("Club no válido");
            var sede = await _sedeRepositorio.GetSedeAsync(idSede, idClub);
            if (sede == null)
            {
                return NotFound();
            }

            var sedeDto = _mapper.Map<SedeDto>(sede);
            return Ok(sedeDto);
        }

        [HttpGet(Name = "GetSedes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSedes()
        {
            int idClub = UsuarioClubId;
            if (idClub <= 0) return Unauthorized("Club no válido");
            var listaSedes = await _sedeRepositorio.GetSedesAsync(idClub);
            var listaSedesDto = _mapper.Map<IEnumerable<SedeDto>>(listaSedes);
            return Ok(listaSedesDto);
        }

        [HttpPut("{idSede:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActualizarSede(int idSede, [FromBody] ModificarSedeDto modificarSedeDto)
        {
            int idClub = UsuarioClubId;
            if (!ModelState.IsValid || modificarSedeDto == null) return BadRequest(ModelState);
            if (idClub <= 0) return Unauthorized("Club no válido");

            var sede = await _sedeRepositorio.GetSedeAsync(idSede, idClub);
            if (sede == null) return NotFound($"No se encontró la sede con id : {idSede}");

            if (!sede.activo) return BadRequest("No se puede actualizar una sede inactiva.");

            _mapper.Map(modificarSedeDto, sede);

            if (!await _sedeRepositorio.ActualizarSedeAsync(sede))
            {
                return StatusCode(500, $"Error al actualizar la sede con id : {idSede}");
            }

            return Ok(sede);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearSede([FromBody] CrearSedeDto crearSedeDto)
        {
            if (!ModelState.IsValid || crearSedeDto == null) return BadRequest(ModelState);
            int idClub = UsuarioClubId;
            if (idClub <= 0) return Unauthorized("Club no válido");

            if (await _sedeRepositorio.ExisteSedeAsync(crearSedeDto.nombreSede, idClub))
            {
                return Conflict("La sede ya existe");
            }

            var sede = _mapper.Map<Sede>(crearSedeDto);

            if (!await _sedeRepositorio.CrearSedeAsync(sede))
            {
                return StatusCode(500, "Error al crear la sede");
            }

            return CreatedAtRoute("GetSede", new { idSede = sede.idSede }, sede);
        }

        [HttpDelete("{idSede:int}")]
        public async Task<IActionResult> BorrarSede(int idSede)
        {
            int idClub = UsuarioClubId;
            if (idClub <= 0) return Unauthorized("Club no válido");
            var sede = await _sedeRepositorio.GetSedeAsync(idSede, idClub);
            if (sede == null) return NotFound();

            var result = await _sedeRepositorio.EliminarSedeAsync(sede);

            return result switch
            {
                ResultadoBorradoSede.TienePistasActivas => BadRequest("No se puede eliminar la sede porque tiene pistas activas."),
                ResultadoBorradoSede.TieneReservasPagadas => BadRequest("No se puede eliminar la sede porque tiene reservas pagadas."),
                ResultadoBorradoSede.ErrorServidor => StatusCode(500, "Error interno al procesar el borrado."),
                ResultadoBorradoSede.Exito => NoContent(),
                _ => StatusCode(500)
            };
        }
    }
}