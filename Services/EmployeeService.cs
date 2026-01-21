using Microsoft.EntityFrameworkCore;
using PayrollApi.Data;
using PayrollApi.DTOs;
using PayrollApi.Helpers;
using PayrollApi.Models;

namespace PayrollApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly PayrollDbContext _context;

        public EmployeeService(PayrollDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employees
                .Include(e => e.WorkingDays)
                .ToListAsync();

            return employees.Select(EmployeeResponseDto.FromEntity);
        }

        public async Task<EmployeeResponseDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.WorkingDays)
                .FirstOrDefaultAsync(e => e.Id == id);

            return employee == null ? null : EmployeeResponseDto.FromEntity(employee);
        }

        public async Task<EmployeeResponseDto> CreateEmployeeAsync(EmployeeCreateDto dto)
        {
            var employeeNumber = EmployeeNumberGenerator.Generate(dto.LastName, dto.DateOfBirth);

            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                DailyRate = dto.DailyRate,
                EmployeeNumber = employeeNumber
            };

            employee.WorkingDays = BuildWorkingDaysFromNumbers(employee, dto.WorkingDayNumbers);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return EmployeeResponseDto.FromEntity(employee);
        }

        public async Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto dto)
        {
            var employee = await _context.Employees
                .Include(e => e.WorkingDays)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return false;
            }

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.DateOfBirth = dto.DateOfBirth;
            employee.DailyRate = dto.DailyRate;

            employee.EmployeeNumber = EmployeeNumberGenerator.Generate(employee.LastName, employee.DateOfBirth);

            _context.EmployeeWorkingDays.RemoveRange(employee.WorkingDays);
            employee.WorkingDays = BuildWorkingDaysFromNumbers(employee, dto.WorkingDayNumbers);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.WorkingDays)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return false;
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return true;
        }

        private static ICollection<EmployeeWorkingDay> BuildWorkingDaysFromNumbers(Employee employee, IEnumerable<int> dayNumbers)
        {
            var days = new List<EmployeeWorkingDay>();

            foreach (var number in dayNumbers.Distinct().OrderBy(n => n))
            {
                if (number < 1 || number > 7)
                {
                    continue;
                }

                var dayOfWeek = DayOfWeekFromNumber(number);
                days.Add(new EmployeeWorkingDay
                {
                    Employee = employee,
                    DayNumber = number,
                    DayName = dayOfWeek.ToString()
                });
            }

            return days;
        }

        private static DayOfWeek DayOfWeekFromNumber(int number)
        {
            return number switch
            {
                1 => DayOfWeek.Monday,
                2 => DayOfWeek.Tuesday,
                3 => DayOfWeek.Wednesday,
                4 => DayOfWeek.Thursday,
                5 => DayOfWeek.Friday,
                6 => DayOfWeek.Saturday,
                7 => DayOfWeek.Sunday,
                _ => throw new ArgumentOutOfRangeException(nameof(number), "Day number must be between 1 and 7.")
            };
        }
    }
}
