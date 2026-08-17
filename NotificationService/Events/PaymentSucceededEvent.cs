namespace PaymentBackend.Domain.Events;
public record PaymentSucceededEvent(Guid TransactionId, Guid OrderId, decimal Amount, DateTime OccurredOn);