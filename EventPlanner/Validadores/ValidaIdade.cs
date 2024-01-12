namespace EventPlanner.Validadores
{
    public abstract class ValidaIdade
    {
        public static bool ValidarIdade(int Idade)
        {
            if (Idade <= 0 || Idade >= 120) { return false; }
            if (Idade <= 18) { return false; }

            return true;
        }
    }
}