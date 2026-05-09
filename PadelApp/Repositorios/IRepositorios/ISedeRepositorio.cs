using PadelApp.Modelos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface ISedeRepositorio
    {
        Task<Sede> GetSedeAsync(int idSede, int idClub);
        Task<IEnumerable<Sede>> GetSedesAsync(int idClub);
        Task<bool> ExisteSedeAsync(int idSede, int idClub);
        Task<bool> ExisteSedeAsync(string nombreSede, int idClub);
        Task<bool> CrearSedeAsync(Sede sede);
        Task<bool> ActualizarSedeAsync(Sede sede);
        Task<ResultadoBorradoSede> EliminarSedeAsync(Sede sede);
        Task<bool> GuardarAsync();
    }
}
