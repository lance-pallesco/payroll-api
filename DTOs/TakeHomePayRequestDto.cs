namespace PayrollApi.DTOs
{
    public record TakeHomePayRequestDto(
        DateTime StartDate,
        DateTime EndDate
    );
}
