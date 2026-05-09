using PadelApp.Modelos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IClubRepositorio
    {
        Task<IEnumerable<Club>> GetClubesPorEmailAsync(string email);
        Task<Club> GetClubByIdAsync(int idClub);
        Task<IEnumerable<Club>> GetClubesAsync();
        Task<bool> ActualizarClubAsync(Club club);
        Task<bool> GuardarAsync();
    }
}
