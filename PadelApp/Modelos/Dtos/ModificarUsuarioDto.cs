using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class ModificarUsuarioDto
    {   
        [Required]
        public string nombre { get; set; }
        [Required]
        public string apellidos { get; set; }
        [Required]
        public string dni_nie { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }        
        public DateOnly fecha_nacimiento { get; set; }       
    }
}
