using PayrollApi.DTOs;

namespace PayrollApi.Services
{
    public interface IPayrollService
    {
        Task<TakeHomePayResponseDto?> CalculateTakeHomePayAsync(int employeeId, TakeHomePayRequestDto request);
    }
}
