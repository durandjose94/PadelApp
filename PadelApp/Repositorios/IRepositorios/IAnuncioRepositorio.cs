using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IAnuncioRepositorio
    {
        Task<IEnumerable<Anuncio>> GetAnunciosActivosAsync();
        Task<IEnumerable<Anuncio>> GetAnunciosByUsuarioAsync(int idUsuario);
        Task<Anuncio> GetAnuncioByIdAsync(int idAnuncio);
        Task<Anuncio> CrearAnuncioAsync(Anuncio anuncio);
        Task<bool> ActualizarAnuncioAsync(Anuncio anuncio);
        Task<bool> EliminarAnuncioAsync(int idAnuncio);
        Task<int> CountAnunciosActivosUsuarioAsync(int idUsuario, TipoAnuncio tipo);
    }
}
