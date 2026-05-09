using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IUsuarioRepositorio
    {
        Task<bool> ExisteUsuarioAsync(int idUsuario, int idClub);
        Task<bool> ExisteEmailAsync(string email);
        Task<Usuario> GetUsuarioAsync(int idUsuario, int idClub);
        Task<Usuario> GetUsuarioAsync(string email, int idClub);
        Task<IEnumerable<Usuario>> GetUsuariosAsync(int idClub);
        Task<bool> UsuarioUnicoAsync(string email, int idClub);
        Task<IEnumerable<Club>> GetClubesPorEmailAsync(string email);
        Task<Usuario> RegistrarUsuarioAsync(Usuario usuario);
        Task<bool> ModificarUsuarioAsync(Usuario usuario);
        Task<ResultadoBorradoUsuario> EliminarUsuarioAsync(Usuario usuario);
        Task<IEnumerable<Usuario>> GetUsuariosConFiltroAsync(string filtro, int idClub);
        Task<bool> ActualizarPasswordAsync(string email, int idClub, string nuevaPassword);
        Task<bool> GuardarAsync();
    }
}
