namespace PadelApp.Helpers
{
    public static class PasswordEncripted
    {
        // Genera un hash seguro con un "Salt" automático
        public static string EncriptarPassword(string password)
        {
            // El "Salt" ya viene incluido en el hash resultante
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Compara la contraseña en texto plano con el hash de la BD
        public static bool VerificarPassword(string passwordDto, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(passwordDto, passwordHash);
        }
    }
}
