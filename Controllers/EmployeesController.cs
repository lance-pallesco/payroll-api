 using Microsoft.AspNetCore.Mvc;
 using Microsoft.EntityFrameworkCore;
 using PayrollApi.Data;
 using PayrollApi.Helpers;
 using PayrollApi.Models;
 
 namespace PayrollApi.Controllers
 {
     [ApiController]
     [Route("api/[controller]")]
     public class EmployeesController : ControllerBase
     {
         private readonly PayrollDbContext _context;
 
         public EmployeesController(PayrollDbContext context)
         {
             _context = context;
         }
 
         // DTOs
         public record EmployeeCreateDto(
             string FirstName,
             string LastName,
             DateTime DateOfBirth,
             decimal DailyRate,
             IEnumerable<int> WorkingDayNumbers // e.g. [1,2,3] for Mon, Tue, Wed
         );
 
         public record EmployeeUpdateDto(
             string FirstName,
             string LastName,
             DateTime DateOfBirth,
             decimal DailyRate,
             IEnumerable<int> WorkingDayNumbers
         );
 
         public record TakeHomePayRequest(
             DateTime StartDate,
             DateTime EndDate
         );
 
         public record TakeHomePayResponse(
             string EmployeeNumber,
             DateTime StartDate,
             DateTime EndDate,
             decimal TakeHomePay
         );
 
         // GET: api/employees
         [HttpGet]
         public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
         {
             var employees = await _context.Employees
                 .Include(e => e.WorkingDays)
                 .ToListAsync();
 
             return Ok(employees);
         }
 
         // GET: api/employees/{id}
         [HttpGet("{id:int}")]
         public async Task<ActionResult<Employee>> GetEmployee(int id)
         {
             var employee = await _context.Employees
                 .Include(e => e.WorkingDays)
                 .FirstOrDefaultAsync(e => e.Id == id);
 
             if (employee == null)
             {
                 return NotFound();
             }
 
             return Ok(employee);
         }
 
         // POST: api/employees
         [HttpPost]
         public async Task<ActionResult<Employee>> CreateEmployee([FromBody] EmployeeCreateDto dto)
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
 
             // Add working days based on the provided day numbers
             employee.WorkingDays = BuildWorkingDaysFromNumbers(employee, dto.WorkingDayNumbers);
 
             _context.Employees.Add(employee);
             await _context.SaveChangesAsync();
 
             return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
         }
 
         // PUT: api/employees/{id}
         [HttpPut("{id:int}")]
         public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto dto)
         {
             var employee = await _context.Employees
                 .Include(e => e.WorkingDays)
                 .FirstOrDefaultAsync(e => e.Id == id);
 
             if (employee == null)
             {
                 return NotFound();
             }
 
             employee.FirstName = dto.FirstName;
             employee.LastName = dto.LastName;
             employee.DateOfBirth = dto.DateOfBirth;
             employee.DailyRate = dto.DailyRate;
 
             // Regenerate employee number if name or DOB changes
             employee.EmployeeNumber = EmployeeNumberGenerator.Generate(employee.LastName, employee.DateOfBirth);
 
             // Replace working days based on new numbers
             _context.EmployeeWorkingDays.RemoveRange(employee.WorkingDays);
             employee.WorkingDays = BuildWorkingDaysFromNumbers(employee, dto.WorkingDayNumbers);
 
             await _context.SaveChangesAsync();
             return NoContent();
         }
 
         // DELETE: api/employees/{id}
         [HttpDelete("{id:int}")]
         public async Task<IActionResult> DeleteEmployee(int id)
         {
             var employee = await _context.Employees
                 .Include(e => e.WorkingDays)
                 .FirstOrDefaultAsync(e => e.Id == id);
 
             if (employee == null)
             {
                 return NotFound();
             }
 
             _context.Employees.Remove(employee);
             await _context.SaveChangesAsync();
 
             return NoContent();
         }
 
         // POST: api/employees/{id}/compute-pay
         [HttpPost("{id:int}/compute-pay")]
         public async Task<ActionResult<TakeHomePayResponse>> ComputeTakeHomePay(
             int id,
             [FromBody] TakeHomePayRequest request)
         {
             if (request.EndDate < request.StartDate)
             {
                 return BadRequest("EndDate must be greater than or equal to StartDate.");
             }
 
             var employee = await _context.Employees
                 .Include(e => e.WorkingDays)
                 .FirstOrDefaultAsync(e => e.Id == id);
 
             if (employee == null)
             {
                 return NotFound();
             }
 
             var takeHomePay = CalculateTakeHomePay(employee, request.StartDate.Date, request.EndDate.Date);
 
             var response = new TakeHomePayResponse(
                 employee.EmployeeNumber,
                 request.StartDate.Date,
                 request.EndDate.Date,
                 takeHomePay);
 
             return Ok(response);
         }
 
         private static ICollection<EmployeeWorkingDay> BuildWorkingDaysFromNumbers(Employee employee, IEnumerable<int> dayNumbers)
         {
             var days = new List<EmployeeWorkingDay>();
 
             foreach (var number in dayNumbers.Distinct().OrderBy(n => n))
             {
                 if (number < 1 || number > 7)
                 {
                     continue; // ignore invalid day numbers (0 = none, 1-7 = Monday-Sunday)
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
             // 1 = Monday ... 7 = Sunday
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
 
         // Business logic for computing take-home pay
         private static decimal CalculateTakeHomePay(Employee employee, DateTime startDate, DateTime endDate)
         {
             var workingDayNumbers = employee.WorkingDays
                 .Select(d => d.DayNumber)
                 .ToHashSet();
 
             decimal total = 0m;
             var current = startDate;
 
             while (current <= endDate)
             {
                 // Map System.DayOfWeek to 1-7 (Monday-Sunday)
                 var dowNumber = ((int)current.DayOfWeek + 6) % 7 + 1;
 
                 bool isWorkingDay = workingDayNumbers.Contains(dowNumber);
                 bool isBirthday = current.Month == employee.DateOfBirth.Month &&
                                   current.Day == employee.DateOfBirth.Day;
 
                 // Twice the daily rate on working days
                 if (isWorkingDay)
                 {
                     total += employee.DailyRate * 2;
                 }
 
                 // 100% of daily rate on birthday, whether working or not
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
 
