using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class CrearReservaDto
    {
        [Required]
        public DateOnly fecha_reserva { get; set; }
        [Required]
        public TimeOnly hora_inicio { get; set; }
        [Required]
        public TimeOnly hora_fin { get; set; }
        [Required]
        public EstadoReserva estado { get; set; }
        [Required]
        public decimal precio { get; set; }
        public string comentarios { get; set; }
        public int idUsuario { get; set; }
        public int idPista { get; set; }
    }
}
