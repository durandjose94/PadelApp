using PadelApp.Modelos;

namespace PadelApp.Repositorios.IRepositorios
{
    public interface IReservaRepositorio
    {
        Task<IEnumerable<Reserva>> GetReservasAsync(int idClub);
        Task<IEnumerable<Reserva>> GetReservasPorUsuarioAsync(int idUsuario, int idClub);
        Task<IEnumerable<Reserva>> GetReservasPorSedeAsync(int idSede, int idClub);
        Task<Reserva> GetReservaAsync(int idReserva, int idClub);
        Task<bool> ExisteReservaAsync(int idReserva, int idClub);
        Task<bool> CrearReservaAsync(Reserva reserva);
        Task<bool> ActualizarReservaAsync(Reserva reserva);
        Task<bool> CancelarReservaAsync(int idReserva);
        Task<bool> GuardarAsync();
        Task<IEnumerable<Reserva>> GetReservasPorPistaYFechaAsync(int idPista, DateOnly fecha, int idClub);
        Task<IEnumerable<Reserva>> GetReservasConFiltroAsync(string filtro, int idSede, int idClub);
    }
}
