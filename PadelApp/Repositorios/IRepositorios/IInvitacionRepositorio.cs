using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IInvitacionRepositorio
    {
        Task<InvitacionClub> CrearInvitacionAsync(CrearInvitacionDto dto);
        Task<InvitacionClub> ValidarInvitacionAsync(string email, string codigo);
        Task<bool> MarcarComoUsadaAsync(string codigo);
        //Task<int?> ObtenerClubPorInvitacionValidaAsync(string email, string codigo);
    }
}
