namespace PaymentBackend.Domain.Events;

public class PaymentFailedEvent
{
    public Guid TransactionId { get; }
    public Guid OrderId { get; }
    public decimal Amount { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }
    public PaymentFailedEvent(Guid transactionId, Guid orderId, decimal amount, string reason)
    {
        TransactionId = transactionId;
        OrderId = orderId;
        Amount = amount;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
}