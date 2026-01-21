using Microsoft.AspNetCore.Mvc;
using PayrollApi.DTOs;
using PayrollApi.Services;

namespace PayrollApi.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IPayrollService _payrollService;

        public EmployeesController(IEmployeeService employeeService, IPayrollService payrollService)
        {
            _employeeService = employeeService;
            _payrollService = payrollService;
        }

        // GET: api/employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // GET: api/employees/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeResponseDto>> GetEmployee(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee([FromBody] EmployeeCreateDto dto)
        {
            var employee = await _employeeService.CreateEmployeeAsync(dto);
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
        }

        // PUT: api/employees/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var updated = await _employeeService.UpdateEmployeeAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deleted = await _employeeService.DeleteEmployeeAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/employees/{id}/compute-pay
        [HttpPost("{id:int}/compute-pay")]
        public async Task<ActionResult<TakeHomePayResponseDto>> ComputeTakeHomePay(
            int id,
            [FromBody] TakeHomePayRequestDto request)
        {
            try
            {
                var response = await _payrollService.CalculateTakeHomePayAsync(id, request);

                if (response == null)
                {
                    return NotFound();
                }

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
