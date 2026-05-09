using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PadelApp.Modelos.Dtos;
using PadelApp.Repositorios.IRepositorios;

namespace PadelApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IClubRepositorio _clubRepo;
        private readonly IMapper _mapper;

        public ClubController(IClubRepositorio clubRepo, IMapper mapper)
        {
            _clubRepo = clubRepo;
            _mapper = mapper;
        }
        
        [AllowAnonymous]
        [HttpGet("disponibles/{email}")]
        public async Task<IActionResult> GetClubesDisponibles(string email)
        {
            var clubes = await _clubRepo.GetClubesPorEmailAsync(email);

            if (clubes == null || !clubes.Any())
            {
                return NotFound(new { mensaje = "No hay cuentas asociadas a este correo." });
            }
            
            var clubesDto = _mapper.Map<IEnumerable<ClubDto>>(clubes);
            return Ok(clubesDto);
        }
        
        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetClub(int id)
        {
            var club = await _clubRepo.GetClubByIdAsync(id);
            if (club == null) return NotFound();

            return Ok(_mapper.Map<ClubDto>(club));
        }
    }
}
