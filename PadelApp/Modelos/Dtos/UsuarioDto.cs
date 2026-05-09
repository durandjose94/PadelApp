using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PadelApp.Modelos.Dtos
{
    public class UsuarioDto
    {
        public int idUsuario { get; set; }
        [Required]
        public string nombre { get; set; }
        [Required]
        public string apellidos { get; set; }
        [Required]
        public string dni_nie { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public DateOnly fecha_nacimiento { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
        public bool activo { get; set; }
        public NivelPadel nivel { get; set; }
        public int idRol { get; set; }
        public int idClub { get; set; }
        public string nombreClub { get; set; }
    }
}
