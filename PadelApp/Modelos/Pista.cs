using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PadelApp.Modelos
{
    public enum EstadoPista
    {
        [Display(Name = "Disponible")]
        Disponible = 1,
        [Display(Name = "No Disponible")]
        NoDisponible = 2,
    }

    public class Pista
    {
        [Key]
        public int idPista { get; set; }
        [Required]
        public string nombrePista { get; set; }
        public EstadoPista estado { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
        public string comentarios { get; set; }
        public int idSede { get; set; }
        [ForeignKey("idSede")]
        public Sede Sede { get; set; }
    }
}
