using Microsoft.EntityFrameworkCore;
using PayrollApi.Data;
using PayrollApi.DTOs;
using PayrollApi.Models;

namespace PayrollApi.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly PayrollDbContext _context;

        public PayrollService(PayrollDbContext context)
        {
            _context = context;
        }

        public async Task<TakeHomePayResponseDto?> CalculateTakeHomePayAsync(int employeeId, TakeHomePayRequestDto request)
        {
            if (request.EndDate < request.StartDate)
            {
                throw new ArgumentException("EndDate must be greater than or equal to StartDate.");
            }

            var employee = await _context.Employees
                .Include(e => e.WorkingDays)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return null;
            }

            var takeHomePay = CalculateTakeHomePay(employee, request.StartDate.Date, request.EndDate.Date);

            return new TakeHomePayResponseDto(
                employee.EmployeeNumber,
                request.StartDate.Date,
                request.EndDate.Date,
                takeHomePay
            );
        }

        private static decimal CalculateTakeHomePay(Employee employee, DateTime startDate, DateTime endDate)
        {
            var workingDayNumbers = employee.WorkingDays
                .Select(d => d.DayNumber)
                .ToHashSet();

            decimal total = 0m;
            var current = startDate;

            while (current <= endDate)
            {
                var dowNumber = ((int)current.DayOfWeek + 6) % 7 + 1;

                bool isWorkingDay = workingDayNumbers.Contains(dowNumber);
                bool isBirthday = current.Month == employee.DateOfBirth.Month &&
                                  current.Day == employee.DateOfBirth.Day;

                if (isWorkingDay)
                {
                    total += employee.DailyRate * 2;
                }

                if (isBirthday)
                {
                    total += employee.DailyRate;
                }

                current = current.AddDays(1);
            }

            return total;
        }
    }
}
