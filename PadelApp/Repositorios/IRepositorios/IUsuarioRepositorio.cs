using PadelApp.Modelos;
using PadelApp.Modelos.Dtos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IUsuarioRepositorio
    {
        Task<bool> ExisteUsuarioAsync(int idUsuario);
        Task<Usuario> GetUsuarioAsync(int idUsuario);
        Task<Usuario> GetUsuarioAsync(string email);
        Task<IEnumerable<Usuario>> GetUsuariosAsync();
        Task<bool> UsuarioUnicoAsync(string email);
        Task<Usuario> RegistrarUsuarioAsync(Usuario usuario);
        Task<bool> ModificarUsuarioAsync(Usuario usuario);
        Task<ResultadoBorradoUsuario> EliminarUsuarioAsync(Usuario usuario);
        Task<IEnumerable<Usuario>> GetUsuariosConFiltroAsync(string filtro);
        Task<bool> ActualizarPasswordAsync(string email, string nuevaPassword);
        Task<bool> GuardarAsync();
    }
}
