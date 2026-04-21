using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PadelApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {
    }
}
