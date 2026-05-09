namespace PadelApp.Modelos.Dtos
{
    public record SolicitarRecuperacionDto(string Email);
    public record VerificarCodigoDto(string Email, string Codigo);
    public record ResetPasswordDto(string Email, string Codigo, string NuevaPassword, int idClub);
}
