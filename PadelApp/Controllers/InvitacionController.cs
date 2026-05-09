using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PadelApp.Modelos.Dtos;
using PadelApp.Repositorios.IRepositorios;

namespace PadelApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitacionController : ControllerBase
    {
        private readonly IInvitacionRepositorio _invitacionRepo;

        public InvitacionController(IInvitacionRepositorio invitacionRepo)
        {
            _invitacionRepo = invitacionRepo;
        }

        [HttpPost("generar")]
        public async Task<IActionResult> GenerarInvitacion([FromBody] CrearInvitacionDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var resultado = await _invitacionRepo.CrearInvitacionAsync(dto);
                
                return Ok(new
                {
                    message = "Invitación enviada con éxito",
                    codigo = resultado.Codigo,
                    expiracion = resultado.FechaExpiracion
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
