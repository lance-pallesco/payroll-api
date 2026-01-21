using PayrollApi.DTOs;

namespace PayrollApi.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync();
        Task<EmployeeResponseDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeResponseDto> CreateEmployeeAsync(EmployeeCreateDto dto);
        Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateDto dto);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
