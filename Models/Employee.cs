using System.Collections.Generic;

namespace PayrollApi.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        public string EmployeeNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public decimal DailyRate { get; set; }

        // Navigation property: which days of the week this employee normally works
        public ICollection<EmployeeWorkingDay> WorkingDays { get; set; } = new List<EmployeeWorkingDay>();
    }
}
