using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PadelApp.Modelos.Dtos
{
    public class ReservaDto
    {
        public int idReserva { get; set; }
        public DateOnly fecha_reserva { get; set; }
        public TimeOnly hora_inicio { get; set; }
        public TimeOnly hora_fin { get; set; }
        public EstadoReserva estado { get; set; }
        public string estadoDescripcion { get; set; }
        [JsonPropertyName("estaFinalizada")]
        public bool EstaFinalizada
        {
            get
            {
                // Combinamos DateOnly + TimeOnly para tener un DateTime real
                DateTime fechaHoraFin = fecha_reserva.ToDateTime(hora_fin);

                // Solo está finalizada si estaba pagada Y el tiempo ya pasó
                return estado == EstadoReserva.Pagada && fechaHoraFin < DateTime.Now;
            }
        }
        public decimal precio { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
        public string comentarios { get; set; }
        public int idUsuario { get; set; }
        public int idPista { get; set; }
        public string nombrePista { get; set; }
        public string nombreSede { get; set; }
        public string direccionSede { get; set; }
        public string nombreUsuario { get; set; }


    }
}
