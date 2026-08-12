namespace PaymentBackend.Application.Interfaces;

/// <summary>
/// Interface cho Message Broker (Dự trù cho RabbitMQ/Kafka để gọi hệ thống Order, Kế toán)
/// </summary>
public interface IMessageBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
}