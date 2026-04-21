using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos
{
    public class Sede
    {
        [Key]
        public int idSede { get; set; }
        [Required]
        public string nombreSede { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
    }
}
