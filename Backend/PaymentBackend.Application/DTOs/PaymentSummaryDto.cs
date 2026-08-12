namespace PaymentBackend.Application.DTOs;

public class PaymentSummaryDto
{
    public string TransactionId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}