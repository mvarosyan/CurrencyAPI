namespace CurrencyAPI.Helpers
{
    public static class CurrencyValidator
    {
        public static bool IsValid(this string code)
        {
            return !string.IsNullOrWhiteSpace(code)
                   && code.Length == 3
                   && code.All(char.IsLetter);
        }
    }
}
