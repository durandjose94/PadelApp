using PadelApp.Modelos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface ISedeRepositorio
    {
        Task<Sede> GetSedeAsync(int idSede);
        Task<IEnumerable<Sede>> GetSedesAsync();
        Task<bool> ExisteSedeAsync(int idSede);
        Task<bool> ExisteSedeAsync(string nombreSede);
        Task<bool> CrearSedeAsync(Sede sede);
        Task<bool> ActualizarSedeAsync(Sede sede);
        Task<ResultadoBorradoSede> EliminarSedeAsync(Sede sede);
        Task<bool> GuardarAsync();
    }
}
