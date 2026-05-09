using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos
{
    public class Club
    {
        [Key]
        public int idClub { get; set; }

        [Required]
        public string nombreClub { get; set; }

        [Required]
        public string cif { get; set; }

        public bool activo { get; set; } = true;
        public DateTime fecha_registro { get; set; }
        public DateTime? fecha_actualizacion { get; set; }
    }
}
