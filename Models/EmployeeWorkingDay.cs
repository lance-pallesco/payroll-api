namespace PayrollApi.Models
{
    public class EmployeeWorkingDay
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        // 0 = none, 1-7 = Monday-Sunday
        public int DayNumber { get; set; }
        public string DayName { get; set; } = string.Empty;
    }
}
