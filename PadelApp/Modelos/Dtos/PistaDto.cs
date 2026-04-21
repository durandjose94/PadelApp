using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PadelApp.Modelos.Dtos
{
    public class PistaDto
    {
        public int idPista { get; set; }
        [Required]
        public string nombrePista { get; set; }
        public EstadoPista estado { get; set; }
        public string estadoDescripcion { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
        public string comentarios { get; set; }
        public int idSede { get; set; }
        public string nombreSede { get; set; }
        public string direccionSede { get; set; }
    }
}
