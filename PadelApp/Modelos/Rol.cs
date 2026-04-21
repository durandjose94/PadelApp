using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos
{
    public class Rol
    {
        [Key]
        public int idRol { get; set; }
        [Required]
        public required string nombreRol { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
    }
}
