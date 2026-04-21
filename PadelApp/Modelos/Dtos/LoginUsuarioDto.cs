using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class LoginUsuarioDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        public string email { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string password { get; set; }
    }
}
