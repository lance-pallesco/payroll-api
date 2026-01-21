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
        public ICollection<EmployeeWorkingDay> WorkingDays { get; set; } = new List<EmployeeWorkingDay>();
    }
}
