using System.Text.RegularExpressions;

namespace EventPlanner.Validadores
{
    public abstract class ValidaSenha
    {
        public static string ValidarSenha(string senha)
        {
            // Verifica se a senha é pelo menos 8 caracteres de comprimento
            if (senha.Length < 8)
            {
                return "A senha deve ter pelo menos 8 caracteres.";
            }

            // Verifica se a senha contém pelo menos um caractere especial
            if (!Regex.IsMatch(senha, @"[!@#$%&*]"))
            {
                return "A senha deve conter pelo menos um caractere especial (!@#$%&*).";
            }

            // Verifica se a senha não é uma senha comum
            var senhasComuns = new string[] { "123456", "abcde", "senhafraca" };
            if (senhasComuns.Contains(senha))
            {
                return "A senha é muito comum. Escolha uma senha mais segura.";
            }

            // Se todas as verificações passarem, a senha é válida
            return null;
        }
    }
}