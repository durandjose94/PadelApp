using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int idClub { get; set; }

        [ForeignKey("idClub")]
        public Club Club { get; set; }
    }
}
