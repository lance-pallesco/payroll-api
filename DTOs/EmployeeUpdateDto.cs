namespace PayrollApi.DTOs
{
    public record EmployeeUpdateDto(
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        decimal DailyRate,
        IEnumerable<int> WorkingDayNumbers
    );
}
