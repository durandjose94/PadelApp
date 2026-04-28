using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PadelApp.Modelos
{
    public enum TipoAnuncio
    {
        Partido = 0,
        Clases = 1
    }

    [Table("Anuncios")]
    public class Anuncio
    {
        [Key]
        public int idAnuncio { get; set; }

        [Required]
        public int idUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuario usuario { get; set; }

        [Required]
        [StringLength(20)]
        public TipoAnuncio tipoAnuncio { get; set; } // PARTIDO, CLASES.

        [Required]
        [StringLength(100)]
        public string titulo { get; set; }

        public string descripcion { get; set; }

        [Column(TypeName = "decimal(2, 1)")]
        public decimal? nivelRequerido { get; set; }

        public DateTime? fechaEvento { get; set; }

        [Required]
        public DateTime fechaExpiracion { get; set; }

        [StringLength(15)]
        public string telefonoContacto { get; set; }

        [Required]
        public bool permiteWhatsapp { get; set; } = true;

        [Required]
        public bool permiteLlamada { get; set; } = false;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? precio { get; set; }
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
    }
}
