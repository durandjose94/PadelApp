using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class CrearAnuncioDto
    {
        // El idUsuario lo solemos sacar del Token o del Auth en el Controller, 
        // pero si lo pasas por el body, inclúyelo aquí:
        [Required]
        public int idUsuario { get; set; }

        [Required]
        public TipoAnuncio tipoAnuncio { get; set; }

        [Required]
        [StringLength(100)]
        public string titulo { get; set; }

        public string descripcion { get; set; }

        public decimal? nivelRequerido { get; set; }
        public DateTime? fechaEvento { get; set; }

        [StringLength(15)]
        public string telefonoContacto { get; set; }

        public bool permiteWhatsapp { get; set; } = true;
        public bool permiteLlamada { get; set; } = false;
        public decimal? precio { get; set; }
    }
}
