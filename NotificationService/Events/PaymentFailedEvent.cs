namespace PaymentBackend.Domain.Events;
public record PaymentFailedEvent(Guid TransactionId, Guid OrderId, decimal Amount, string FailureReason, DateTime OccurredOn);