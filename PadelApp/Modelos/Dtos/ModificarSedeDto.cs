using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class ModificarSedeDto
    {
        [Required(ErrorMessage = "Debe ingresar un nombre")]
        [MaxLength(100, ErrorMessage = "El número máximo de caracteres es 100")]
        public string nombreSede { get; set; }
        [MaxLength(200, ErrorMessage = "El número máximo de caracteres es 200")]
        public string direccion { get; set; }
        public string telefono { get; set; }
    }
}
