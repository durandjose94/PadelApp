using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class RegistroUsuarioDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        public string apellidos { get; set; }
        [Required(ErrorMessage = "El dni/nie es obligatorio")]
        public string dni_nie { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        [Required(ErrorMessage = "El correo es obligatorio")]
        public string email { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string password { get; set; }
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateOnly fecha_nacimiento { get; set; }
    }
}
