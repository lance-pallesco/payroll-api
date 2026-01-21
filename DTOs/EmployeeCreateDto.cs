namespace PayrollApi.DTOs
{
    public record EmployeeCreateDto(
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        decimal DailyRate,
        IEnumerable<int> WorkingDayNumbers
    );
}
