using PadelApp.Modelos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IRecuperarContraseñaRepositorio
    {
        Task GuardarCodigoAsync(RecuperacionPassword recuperacion);
        Task<RecuperacionPassword?> ObtenerCodigoValidoAsync(string email, string codigo);
        Task MarcarComoUsadoAsync(int id);
    }
}
