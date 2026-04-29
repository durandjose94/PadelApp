using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;
using PadelApp.Repositorios.IRepositorios;

namespace PadelApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnuncioController : ControllerBase
    {
        private readonly IAnuncioRepositorio _repoAnuncio;
        private readonly IMapper _mapper;

        public AnuncioController(IAnuncioRepositorio repoAnuncio, IMapper mapper)
        {
            _repoAnuncio = repoAnuncio;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAnunciosActivos()
        {
            var listaAnuncios = await _repoAnuncio.GetAnunciosActivosAsync();
            var listaAnunciosDto = _mapper.Map<IEnumerable<AnuncioDto>>(listaAnuncios);
            return Ok(listaAnunciosDto);
        }

        [HttpGet("mis-anuncios/{idUsuario}")]
        public async Task<IActionResult> GetMisAnuncios(int idUsuario)
        {
            var listaAnuncios = await _repoAnuncio.GetAnunciosByUsuarioAsync(idUsuario);
            var listaAnunciosDto = _mapper.Map<IEnumerable<AnuncioDto>>(listaAnuncios);
            return Ok(listaAnunciosDto);
        }

        [HttpPost]
        public async Task<IActionResult> CrearAnuncio([FromBody] CrearAnuncioDto crearAnuncioDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Control de Spam
            var count = await _repoAnuncio.CountAnunciosActivosUsuarioAsync(crearAnuncioDto.idUsuario, crearAnuncioDto.tipoAnuncio);
            if (count >= 3) return BadRequest(new { message = "Límite de anuncios activos alcanzado." });

            var anuncio = _mapper.Map<Anuncio>(crearAnuncioDto);
            var anuncioCreado = await _repoAnuncio.CrearAnuncioAsync(anuncio);

            return Ok(_mapper.Map<AnuncioDto>(anuncioCreado));
        }

        [HttpPut("{idAnuncio}")]
        public async Task<IActionResult> ActualizarAnuncio(int idAnuncio, [FromBody] ModificarAnuncioDto modificarAnuncioDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var anuncioExistente = await _repoAnuncio.GetAnuncioByIdAsync(idAnuncio);
            if (anuncioExistente == null) return NotFound();

            _mapper.Map(modificarAnuncioDto, anuncioExistente);

            anuncioExistente.idAnuncio = idAnuncio;

            if (!await _repoAnuncio.ActualizarAnuncioAsync(anuncioExistente)) return StatusCode(500);

            return NoContent();
        }

        [HttpDelete("{idAnuncio}")]
        public async Task<IActionResult> EliminarAnuncio(int idAnuncio)
        {
            if (!await _repoAnuncio.EliminarAnuncioAsync(idAnuncio)) return NotFound();
            return NoContent();
        }

        [HttpGet("{idAnuncio:int}", Name = "GetAnuncio")]
        public async Task<IActionResult> GetAnuncio(int idAnuncio)
        {
            var anuncio = await _repoAnuncio.GetAnuncioByIdAsync(idAnuncio);

            if (anuncio == null)
            {
                return NotFound(new { message = "El anuncio no existe." });
            }

            var anuncioDto = _mapper.Map<AnuncioDto>(anuncio);

            return Ok(anuncioDto);
        }
    }
}
