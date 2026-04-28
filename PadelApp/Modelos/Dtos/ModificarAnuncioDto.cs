using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class ModificarAnuncioDto
    {
        [Required]
        public int idAnuncio { get; set; } // Necesario para saber cuál editamos

        [Required]
        [StringLength(100)]
        public string titulo { get; set; }

        public string descripcion { get; set; }

        public decimal? nivelRequerido { get; set; }
        public DateTime? fechaEvento { get; set; }

        [StringLength(15)]
        public string telefonoContacto { get; set; }

        public bool permiteWhatsapp { get; set; }
        public bool permiteLlamada { get; set; }
        public decimal? precio { get; set; }
    }
}
