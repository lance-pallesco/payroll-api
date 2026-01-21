namespace PayrollApi.Models
{
    public class EmployeeWorkingDay
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public int DayNumber { get; set; }
        public string DayName { get; set; } = string.Empty;
    }
}
