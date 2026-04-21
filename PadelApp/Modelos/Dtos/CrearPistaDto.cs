using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class CrearPistaDto
    {
        [Required]
        public string nombrePista { get; set; }
        public EstadoPista estado { get; set; }
        public bool activo { get; set; } = true;
        public string comentarios { get; set; }
        public int idSede { get; set; }
    }
}
