using MailKit.Net.Smtp;
using MimeKit;
using PadelApp.Servicios.IServicios;

namespace PadelApp.Servicios
{
    public class EmailServicio : IEmailServicio
    {
        private readonly IConfiguration _config;

        public EmailServicio(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarEmailAsync(string emailDestino, string asunto, string mensajeHtml)
        {
            var smtp = _config.GetSection("SmtpSettings");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(smtp["SenderName"], smtp["SenderEmail"]));
            email.To.Add(new MailboxAddress("Usuario PadelApp", emailDestino));
            email.Subject = asunto;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = mensajeHtml
            };

            email.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            try
            {
                // Usamos la misma configuración de servidor y puerto que en tu otro servicio
                await client.ConnectAsync(smtp["Server"], int.Parse(smtp["Port"]), MailKit.Security.SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(smtp["SenderEmail"], smtp["Password"]);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Aquí podrías loguear el error si el envío falla
                throw new Exception("Error al enviar el correo electrónico", ex);
            }
        }
    }
}