using PadelApp.Modelos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IReservaRepositorio
    {
        Task<IEnumerable<Reserva>> GetReservasAsync();
        Task<IEnumerable<Reserva>> GetReservasPorUsuarioAsync(int idUsuario);
        Task<IEnumerable<Reserva>> GetReservasPorSedeAsync(int idSede);
        Task<Reserva> GetReservaAsync(int idReserva);
        Task<bool> ExisteReservaAsync(int idReserva);
        Task<bool> CrearReservaAsync(Reserva reserva);
        Task<bool> ActualizarReservaAsync(Reserva reserva);
        Task<bool> CancelarReservaAsync(int idReserva);
        Task<bool> GuardarAsync();
        Task<IEnumerable<Reserva>> GetReservasPorPistaYFechaAsync(int idPista, DateOnly fecha);
        Task<IEnumerable<Reserva>> GetReservasConFiltroAsync(string filtro, int idSede);
    }
}
