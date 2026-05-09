using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class CrearInvitacionDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int IdClub { get; set; } // El club al que pertenece el admin

        public int? IdUsuarioCreador { get; set; }
    }
}
