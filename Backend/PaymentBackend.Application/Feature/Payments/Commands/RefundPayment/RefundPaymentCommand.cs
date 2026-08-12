namespace PaymentBackend.Application.Features.Payments.Commands.RefundPayment;

public class RefundPaymentCommand
{
    public string TransactionId { get; set; } = string.Empty;
    public decimal RefundAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
}