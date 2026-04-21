using PadelApp.Modelos;

namespace PadelApp.Servicios.IServicios
{
    public interface IComprobanteServicio
    {
        byte[] GenerarPdfReserva(Reserva reserva, Usuario usuario, Pista pista);
        Task EnviarEmailConComprobante(string emailDestino, byte[] pdfBytes, Reserva reserva);
    }
}
