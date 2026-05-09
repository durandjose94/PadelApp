using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class ClubDto
    {
        public int idClub { get; set; }

        [Required]
        public string nombreClub { get; set; }

        [Required]
        public string cif { get; set; }

        public bool activo { get; set; } = true;
    }
}
