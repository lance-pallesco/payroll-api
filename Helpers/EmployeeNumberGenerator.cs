namespace PayrollApi.Helpers
{
    public static class EmployeeNumberGenerator
    {
        public static string Generate(string lastName, DateTime dateOfBirth)
        {
            var prefix = lastName.Substring(0, 3).ToUpper();
            var random = new Random().Next(0, 9999).ToString("D5");
            var datePart = dateOfBirth.ToString("ddMMMyyyy").ToUpper();

            return $"{prefix}-{random}-{datePart}";
        }
    }
}
