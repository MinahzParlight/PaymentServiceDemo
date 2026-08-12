
namespace PaymentBackend.Application.DTOs;

public class PaymentDetailsDto
{
    public string TransactionId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string GatewayReference { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}