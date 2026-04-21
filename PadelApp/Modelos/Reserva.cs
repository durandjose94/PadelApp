using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PadelApp.Modelos
{
    public enum EstadoReserva
    {
        Creada = 1,
        Pagada = 2,
        Cancelada = 3
    }
    public class Reserva
    {
        [Key]
        public int idReserva { get; set; }
        [Required]
        public DateOnly fecha_reserva { get; set; }
        [Required]
        public TimeOnly hora_inicio { get; set; }
        [Required]
        public TimeOnly hora_fin { get; set; }
        [Required]
        public EstadoReserva estado { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal precio { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
        public string comentarios { get; set; }
        public int idUsuario { get; set; }
        [ForeignKey("idUsuario")]
        public Usuario Usuario { get; set; }
        public int idPista { get; set; }
        [ForeignKey("idPista")]
        public Pista Pista { get; set; }
    }
}
