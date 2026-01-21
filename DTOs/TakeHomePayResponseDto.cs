namespace PayrollApi.DTOs
{
    public record TakeHomePayResponseDto(
        string EmployeeNumber,
        DateTime StartDate,
        DateTime EndDate,
        decimal TakeHomePay
    );
}
