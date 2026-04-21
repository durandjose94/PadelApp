namespace PadelApp.Servicios.IServicios
{
    public interface IEmailServicio
    {
        Task EnviarEmailAsync(string emailDestino, string asunto, string mensajeHtml);
    }
}
