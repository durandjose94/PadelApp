using MailKit.Net.Smtp;
using MimeKit;
using PadelApp.Servicios.IServicios;
using System.Text.Json;
using System.Net.Http;
using System.Text;

namespace PadelApp.Servicios
{
    public class EmailServicio : IEmailServicio
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public EmailServicio(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task EnviarEmailAsync(string emailDestino, string asunto, string mensajeHtml)
        {
            var smtp = _config.GetSection("SmtpSettings");

            // Configuramos la petición HTTP hacia Brevo
            var url = smtp["Server"]; // https://api.brevo.com/v3/smtp/email
            var apiKey = smtp["Password"]; // Tu clave de Brevo

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

            // Estructura exacta que pide la API de Brevo
            var payload = new
            {
                sender = new { name = smtp["SenderName"], email = smtp["SenderEmail"] },
                to = new[] { new { email = emailDestino, name = "Usuario PadelApp" } },
                subject = asunto,
                htmlContent = mensajeHtml
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Brevo rechazó el correo: {errorBody}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error crítico al conectar con la API de Brevo", ex);
            }
            /*var smtp = _config.GetSection("SmtpSettings");

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
            }*/
        }
    }
}