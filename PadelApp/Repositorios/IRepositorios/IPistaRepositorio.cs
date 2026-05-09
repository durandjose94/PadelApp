using PadelApp.Modelos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IPistaRepositorio
    {
        Task<Pista> GetPistaAsync(int idPista, int idClub);
        Task<ICollection<Pista>> GetPistasAsync(int idClub);
        Task<ICollection<Pista>> GetPistasPorSedeAsync(int idSede, int idClub);
        Task<bool> ExistePistaAsync(int idPista, int idClub);
        Task<bool> ExistePistaAsync(string nombrePista, int idClub);
        Task<bool> CrearPistaAsync(Pista pista);
        Task<bool> ActualizarPistaAsync(Pista pista);
        Task<ResultadoBorradoPista> EliminarPistaAsync(Pista pista);
        Task<bool> GuardarAsync();
    }
}
