using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PadelApp.Helpers;
using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;
using PadelApp.Repositorios;
using PadelApp.Repositorios.IRepositorios;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PadelApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;
        private readonly string claveSecreta;

        public UsuarioController(IUsuarioRepositorio usuarioRepositorio, IMapper mapper, IConfiguration config)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
        }
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioDto registroDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Verificación de email único asíncrona
            if (!await _usuarioRepositorio.UsuarioUnicoAsync(registroDto.email))
                return BadRequest("El email ya está registrado");

            var usuario = _mapper.Map<Usuario>(registroDto);
            var usuarioCreado = await _usuarioRepositorio.RegistrarUsuarioAsync(usuario);

            if (usuarioCreado == null) return BadRequest("Error al registrar el usuario");

            return Ok(usuarioCreado);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto loginDto)
        {
            // Recuperamos el usuario de forma asíncrona
            var usuario = await _usuarioRepositorio.GetUsuarioAsync(loginDto.email);

            if (usuario == null || !PasswordEncripted.VerificarPassword(loginDto.password, usuario.password))
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            }

            // --- Lógica de Generación de Token ---
            var manejadoToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.idUsuario.ToString()),
                    new Claim(ClaimTypes.Role, usuario.idRol.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(8), //durará todo el día
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manejadoToken.CreateToken(tokenDescriptor);

            return Ok(new RespuestaLoginUsuarioDto
            {
                token = manejadoToken.WriteToken(token),
                //Usuario = usuario
                Usuario = _mapper.Map<UsuarioDto>(usuario) // Mapeamos a DTO para no enviar la password
            });
        }

        [Authorize]
        [HttpPut("{idUsuario:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ModificarUsuario(int idUsuario, [FromBody] ModificarUsuarioDto modificarUsuarioDto)
        {
            if (!ModelState.IsValid || modificarUsuarioDto == null) return BadRequest(ModelState);

            var usuario = await _usuarioRepositorio.GetUsuarioAsync(idUsuario);
            if (usuario == null) return NotFound($"No se encontró el usuario con id : {idUsuario}");

            if (!usuario.activo) return BadRequest("No se puede actualizar un usuario inactivo.");

            _mapper.Map(modificarUsuarioDto, usuario);

            if (!await _usuarioRepositorio.ModificarUsuarioAsync(usuario))
            {
                return StatusCode(500, $"Error al actualizar el usuario con id : {idUsuario}");
            }

            return Ok(usuario);
        }

        [Authorize(Roles = "1")] // Solo administradores
        [HttpDelete("{idUsuario:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BorrarUsuario(int idUsuario)
        {
            var usuario = await _usuarioRepositorio.GetUsuarioAsync(idUsuario);
            if (usuario == null) return NotFound();

            var result = await _usuarioRepositorio.EliminarUsuarioAsync(usuario);

            return result switch
            {
                ResultadoBorradoUsuario.TieneReservasPagadasActivas => BadRequest("No se puede eliminar el usuario porque tiene reservas pagadas activas."),
                ResultadoBorradoUsuario.ErrorServidor => StatusCode(500, "Error interno al procesar el borrado."),
                ResultadoBorradoUsuario.Exito => NoContent(),
                _ => StatusCode(500)
            };
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var listaUsuarios = await _usuarioRepositorio.GetUsuariosAsync();
            var listaUsuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(listaUsuarios);
            return Ok(listaUsuariosDto);
        }

        [Authorize]
        [HttpGet("{idUsuario:int}")]
        public async Task<IActionResult> GetUsuario(int idUsuario)
        {
            var usuario = await _usuarioRepositorio.GetUsuarioAsync(idUsuario);
            if (usuario == null) return NotFound();

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            return Ok(usuarioDto);
        }

        [Authorize]
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuariosFiltrados([FromQuery] string filtro)
        {
            var usuarios = await _usuarioRepositorio.GetUsuariosConFiltroAsync(filtro);

            var usuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(usuarios);

            return Ok(usuariosDto);
        }
    }
}
