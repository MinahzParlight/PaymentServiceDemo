namespace PaymentBackend.Application.Features.Payments.Queries.GetHistoricalPayments;

public class GetHistoricalPaymentsQuery
{
    public string OrderId { get; set; } = string.Empty; // Dùng OrderId làm mốc tra cứu cho Demo
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}