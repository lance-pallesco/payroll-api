using PayrollApi.Models;

namespace PayrollApi.DTOs
{
    public record EmployeeResponseDto(
        int Id,
        string EmployeeNumber,
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        decimal DailyRate,
        IEnumerable<int> WorkingDayNumbers
    )
    {
        public static EmployeeResponseDto FromEntity(Employee employee)
        {
            return new EmployeeResponseDto(
                employee.Id,
                employee.EmployeeNumber,
                employee.FirstName,
                employee.LastName,
                employee.DateOfBirth,
                employee.DailyRate,
                employee.WorkingDays.Select(wd => wd.DayNumber).OrderBy(d => d)
            );
        }
    }
}
