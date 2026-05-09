using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PadelApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PadelControllerBase : ControllerBase
    {
        // Propiedad para obtener el ID del Club directamente
        protected int UsuarioClubId
        {
            get
            {
                var claim = User.FindFirst("idClub");
                return claim != null ? int.Parse(claim.Value) : 0;
            }
        }

        // Propiedad para obtener el ID del Usuario logueado
        protected int UsuarioId
        {
            get
            {
                // En tu login usaste ClaimTypes.Name para el idUsuario
                var claim = User.FindFirst(ClaimTypes.Name);
                return claim != null ? int.Parse(claim.Value) : 0;
            }
        }

        // Propiedad para obtener el Rol
        protected int UsuarioRolId
        {
            get
            {
                var claim = User.FindFirst(ClaimTypes.Role);
                return claim != null ? int.Parse(claim.Value) : 0;
            }
        }
    }
}
