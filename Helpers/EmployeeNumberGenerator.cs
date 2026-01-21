namespace PayrollApi.Helpers
{
    public static class EmployeeNumberGenerator
    {
        public static string Generate(string lastName, DateTime dateOfBirth)
        {
            var prefix = lastName.Length >= 3 
                ? lastName.Substring(0, 3).ToUpper()
                : lastName.ToUpper().PadRight(3, '*');
            
            var random = new Random().Next(0, 100000).ToString("D5");
            var datePart = dateOfBirth.ToString("ddMMMyyyy").ToUpper();

            return $"{prefix}-{random}-{datePart}";
        }
    }
}
