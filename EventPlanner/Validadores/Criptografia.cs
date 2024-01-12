namespace EventPlanner.Validadores
{
    public abstract class Criptografia
    {
        public static string GerarHash(string senha)
        {
            // Gere um salt aleatório
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            // Gere o hash da senha com o salt
            string hash = BCrypt.Net.BCrypt.HashPassword(senha, salt);

            return hash;
        }

        public static bool VerificarSenha(string senha, string hash)
        {
            // Verifique se a senha corresponde ao hash
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }

    }
}
