using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MailKit.Net.Smtp;
using MimeKit;
using PadelApp.Modelos;
using PadelApp.Servicios.IServicios;

namespace PadelApp.Servicios
{
    public class ComprobanteServicio : IComprobanteServicio
    {
        private readonly IConfiguration _config;

        public ComprobanteServicio(IConfiguration config)
        {
            _config = config;
        }
        public byte[] GenerarPdfReserva(Reserva reserva, Usuario usuario, Pista pista)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Verdana));

                    // 1. CABECERA (Igual que antes)
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("PADEL APP").FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text("Comprobante de Pago").FontSize(10).Italic();
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Reserva #{reserva.idReserva}").FontSize(12).SemiBold();
                            col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}");
                        });
                    });

                    // 2. CONTENIDO
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Spacing(20);

                        // Bloque de datos
                        col.Item().Row(row => {
                            row.RelativeItem().Column(c => {
                                c.Item().Text("CLIENTE").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().Text($"{usuario.nombre} {usuario.apellidos}");
                                c.Item().Text(usuario.email);
                            });
                            row.RelativeItem().Column(c => {
                                c.Item().Text("DETALLES DE PISTA").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().Text(pista.nombrePista);
                                c.Item().Text($"Horario: {reserva.hora_inicio:HH:mm} - {reserva.hora_fin:HH:mm}");
                            });
                            row.RelativeItem().Column(c => {
                                c.Item().Text("DETALLES DE SEDE").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().Text(pista.Sede.nombreSede);
                                c.Item().Text(pista.Sede.direccion);
                            });
                        });

                        // 3. TABLA DE CONCEPTOS
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4); // Descripción
                                columns.RelativeColumn();  // Cantidad
                                columns.RelativeColumn();  // Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Concepto");
                                header.Cell().Element(CellStyle).AlignCenter().Text("Sesión");
                                header.Cell().Element(CellStyle).AlignRight().Text("Precio");

                                static IContainer CellStyle(IContainer container) =>
                                    container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            });

                            // Fila del producto
                            table.Cell().Element(ContentStyle).Text($"Alquiler de pista de pádel - {reserva.fecha_reserva:dd/MM/yyyy}");
                            table.Cell().Element(ContentStyle).AlignCenter().Text("1h");
                            table.Cell().Element(ContentStyle).AlignRight().Text($"{reserva.precio:C}");

                            static IContainer ContentStyle(IContainer container) => container.PaddingVertical(8);
                        });

                        // 4. SECCIÓN DE TOTALES (Aquí es donde añadimos el detalle del precio)
                        col.Item().AlignRight().MinWidth(150).Column(totalCol =>
                        {
                            totalCol.Spacing(5);

                            // Subtotal
                            totalCol.Item().Row(row => {
                                row.RelativeItem().Text("Base Imponible:");
                                row.RelativeItem().AlignRight().Text($"{reserva.precio:C}");
                            });

                            // IVA
                            totalCol.Item().Row(row => {
                                row.RelativeItem().Text("IVA (0%):");
                                row.RelativeItem().AlignRight().Text("€0.00");
                            });

                            // Línea divisoria
                            totalCol.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            // TOTAL FINAL
                            totalCol.Item().Row(row => {
                                row.RelativeItem().Text("TOTAL").FontSize(14).SemiBold();
                                row.RelativeItem().AlignRight().Text($"{reserva.precio:C}").FontSize(14).SemiBold().FontColor(Colors.Blue.Medium);
                            });
                        });
                    });

                    // 5. PIE DE PÁGINA
                    page.Footer().AlignCenter().Column(f => {
                        f.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                        f.Item().PaddingTop(5).Text("Este documento sirve como comprobante de pago para el acceso a las instalaciones.").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            }).GeneratePdf();
        }

        public async Task EnviarEmailConComprobante(string emailDestino, byte[] pdfBytes, Reserva reserva)
        {
            var smtp = _config.GetSection("SmtpSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtp["SenderName"], smtp["SenderEmail"]));
            message.To.Add(new MailboxAddress("Jugador", emailDestino));
            message.Subject = $"Confirmación de Reserva #{reserva.idReserva}";

            var body = new BodyBuilder
            {
                HtmlBody = $@"
                <h3>¡Hola! Tu reserva ha sido confirmada con éxito.</h3>
                <p>Adjunto encontrarás el comprobante de tu reserva para el día <b>{reserva.fecha_reserva:dd/MM/yyyy}</b>.</p>
                <p>Recuerda llegar 10 minutos antes de tu turno.</p>
                <br>
                <p>Saludos,<br>El equipo de Padel App</p>"
            };

            body.Attachments.Add($"Comprobante_Reserva_{reserva.idReserva}.pdf", pdfBytes);
            message.Body = body.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtp["Server"], int.Parse(smtp["Port"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtp["SenderEmail"], smtp["Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
