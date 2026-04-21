using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class ModificarPistaDto
    {
        [Required]
        public string nombrePista { get; set; }
        public EstadoPista estado { get; set; }                
        public string comentarios { get; set; }
        public int idSede { get; set; }
    }
}
