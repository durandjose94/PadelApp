using Microsoft.AspNetCore.Mvc;
using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;
using PadelApp.Repositorios;
using PadelApp.Repositorios.IRepositorios;
using PadelApp.Servicios.IServicios;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepositorio _usuarioRepo; // Tu repo de usuarios actual
    private readonly IRecuperarContraseñaRepositorio _recuperacionRepo;
    private readonly IEmailServicio _emailServicio;

    public AuthController(IUsuarioRepositorio usuarioRepo, IRecuperarContraseñaRepositorio recuperacionRepo, IEmailServicio emailServicio)
    {
        _usuarioRepo = usuarioRepo;
        _recuperacionRepo = recuperacionRepo;
        _emailServicio = emailServicio;
    }

    [HttpPost("solicitar-recuperacion")]
    public async Task<IActionResult> Solicitar([FromBody] SolicitarRecuperacionDto dto)
    {
        var usuario = await _usuarioRepo.GetUsuarioAsync(dto.Email);
        if (usuario == null) return Ok(); // Por seguridad, no decimos si el email existe o no

        // 1. Generar código de 6 dígitos
        string codigo = new Random().Next(100000, 999999).ToString();

        // 2. Guardar en BD (expira en 15 min)
        var recuperacion = new RecuperacionPassword
        {
            Email = dto.Email,
            Codigo = codigo,
            FechaExpiracion = DateTime.Now.AddMinutes(15)
        };

        await _recuperacionRepo.GuardarCodigoAsync(recuperacion);

        // --- ENVIAR CORREO GENÉRICO ---
        string asunto = "Recuperar Contraseña - PadelApp";
        string contenido = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
            <h2 style='color: #2c3e50; text-align: center;'>Padel App</h2>
            <p>Hola,</p>
            <p>Has solicitado restablecer tu contraseña. Utiliza el siguiente código para completar el proceso:</p>
            <div style='background-color: #f8f9fa; padding: 20px; text-align: center; font-size: 30px; font-weight: bold; letter-spacing: 10px; color: #3498db; border-radius: 5px; margin: 20px 0;'>
                {codigo}
            </div>
            <p style='color: #7f8c8d; font-size: 12px;'>Este código expirará en 15 minutos por razones de seguridad.</p>
            <p>Si no has solicitado este cambio, puedes ignorar este correo tranquilamente.</p>
            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
            <p style='text-align: center; font-size: 11px; color: #bdc3c7;'>© {DateTime.Now.Year} Padel App - Gestión de Reservas</p>
        </div>";

        await _emailServicio.EnviarEmailAsync(dto.Email, asunto, contenido);

        return Ok(new { message = "Si el correo existe, se ha enviado un código." });
    }

    [HttpPost("verificar-codigo")]
    public async Task<IActionResult> Verificar([FromBody] VerificarCodigoDto dto)
    {
        var valido = await _recuperacionRepo.ObtenerCodigoValidoAsync(dto.Email, dto.Codigo);
        if (valido == null) return BadRequest("Código inválido o expirado.");

        return Ok(new { message = "Código verificado correctamente." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> Reset([FromBody] ResetPasswordDto dto)
    {
        // 1. Validamos que el código sea correcto y no haya expirado
        var registro = await _recuperacionRepo.ObtenerCodigoValidoAsync(dto.Email, dto.Codigo);
        if (registro == null) return BadRequest("Código inválido o expirado.");

        // 2. Usamos el nuevo método específico del repositorio de usuarios
        var actualizado = await _usuarioRepo.ActualizarPasswordAsync(dto.Email, dto.NuevaPassword);

        if (!actualizado)
        {
            return StatusCode(500, "Error al actualizar la contraseña en la base de datos.");
        }

        // 3. Marcamos el código como usado para que no se pueda reutilizar
        await _recuperacionRepo.MarcarComoUsadoAsync(registro.Id);

        return Ok(new { message = "Contraseña actualizada con éxito." });
    }
}