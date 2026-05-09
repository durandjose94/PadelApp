using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PadelApp.Modelos
{
    public enum NivelPadel
    {
        Principiante = 1,
        Iniciacion = 2,
        Intermedio = 3,
        Avanzado = 4,
        Experto = 5,
        Profesional = 6
    }
    public class Usuario
    {

        [Key]
        public int idUsuario { get; set; }
        [Required]
        public string nombre { get; set; }
        [Required]
        public string apellidos { get; set; }
        [Required]
        public string dni_nie { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public DateOnly fecha_nacimiento { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
        public bool activo { get; set; }
        [Required]
        public NivelPadel nivel { get; set; }
        public int idRol { get; set; }
        [ForeignKey("idRol")]
        public Rol Rol { get; set; }
        public int idClub { get; set; }

        [ForeignKey("idClub")]
        public Club Club { get; set; }
    }
}
