using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PadelApp.Modelos
{
    public class InvitacionClub
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(10)] // Por si usas códigos de 6 u 8 caracteres
        public string Codigo { get; set; }

        [Required]
        public int IdClub { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime FechaExpiracion { get; set; }

        public bool Usado { get; set; } = false;

        public int? IdUsuarioCreador { get; set; }

    }
}
