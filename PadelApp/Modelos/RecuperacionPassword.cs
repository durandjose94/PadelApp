using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos
{
    public class RecuperacionPassword
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } // Lo enlazamos por email para facilitar la búsqueda

        [Required]
        public string Codigo { get; set; }

        [Required]
        public DateTime FechaExpiracion { get; set; }

        public bool Usado { get; set; } = false;
    }
}
