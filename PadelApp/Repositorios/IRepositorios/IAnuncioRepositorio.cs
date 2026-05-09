using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IAnuncioRepositorio
    {
        Task<IEnumerable<Anuncio>> GetAnunciosActivosAsync(int idClub);
        Task<IEnumerable<Anuncio>> GetAnunciosByUsuarioAsync(int idUsuario, int idClub);
        Task<Anuncio> GetAnuncioByIdAsync(int idAnuncio, int idClub);
        Task<Anuncio> CrearAnuncioAsync(Anuncio anuncio);
        Task<bool> ActualizarAnuncioAsync(Anuncio anuncio);
        Task<bool> EliminarAnuncioAsync(int idAnuncio);
        Task<int> CountAnunciosActivosUsuarioAsync(int idUsuario, TipoAnuncio tipo, int idClub);
    }
}
