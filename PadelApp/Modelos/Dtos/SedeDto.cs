using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class SedeDto
    {        
        public int idSede { get; set; }
        [Required(ErrorMessage = "Debe ingresar un nombre")]
        [MaxLength(100, ErrorMessage = "El número máximo de caracteres es 100")]
        public string nombreSede { get; set; }
        [MaxLength(200, ErrorMessage = "El número máximo de caracteres es 200")]
        public string direccion { get; set; }
        public string telefono { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime fecha_actualizacion { get; set; }
    }
}
