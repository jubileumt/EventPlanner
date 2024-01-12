using System.Text.RegularExpressions;
namespace EventPlanner.Validadores
{
    public abstract class ValidarTelefone
    {
        public static bool ValidarTeledone(string telefone)
        {
            // Define a regex pattern para números de telefone brasileiros com ou sem separadores
            string pattern = @"^\(?\d{2}\)?[-.\s]?\d{4,5}[-.\s]?\d{4}$";

            // Remove caracteres especiais e espaços em branco para comparar com o regex
            string cleanPhoneNumber = Regex.Replace(telefone, @"[^\d]", "");

            // Verifica se o número limpo corresponde ao padrão brasileiro
            return Regex.IsMatch(cleanPhoneNumber, pattern);

        }
    }
}