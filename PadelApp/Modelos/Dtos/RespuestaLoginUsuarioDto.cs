using System.ComponentModel.DataAnnotations;

namespace PadelApp.Modelos.Dtos
{
    public class RespuestaLoginUsuarioDto
    {
        //public Usuario Usuario { get; set; }
        public UsuarioDto Usuario { get; set; }
        public string rol { get; set; }
        public string token { get; set; }
    }
}
