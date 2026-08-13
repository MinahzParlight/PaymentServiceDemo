
namespace PaymentBackend.Domain.Events;
public class PaymentSucceededEvent
{
    public Guid TransactionId { get; }
    public Guid OrderId { get; }
    public decimal Amount { get; }
    public DateTime OccurredOn { get; }

    public PaymentSucceededEvent(Guid transactionId, Guid orderId, decimal amount)
    {
        TransactionId = transactionId;
        OrderId = orderId;
        Amount = amount;
        OccurredOn = DateTime.UtcNow;
    }
}