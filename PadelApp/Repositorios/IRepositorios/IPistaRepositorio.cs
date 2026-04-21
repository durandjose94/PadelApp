using PadelApp.Modelos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IPistaRepositorio
    {
        Task<Pista> GetPistaAsync(int idPista);
        Task<ICollection<Pista>> GetPistasAsync();
        Task<ICollection<Pista>> GetPistasPorSedeAsync(int idSede);
        Task<bool> ExistePistaAsync(int idPista);
        Task<bool> ExistePistaAsync(string nombrePista);
        Task<bool> CrearPistaAsync(Pista pista);
        Task<bool> ActualizarPistaAsync(Pista pista);
        Task<ResultadoBorradoPista> EliminarPistaAsync(Pista pista);
        Task<bool> GuardarAsync();
    }
}
