using PaymentBackend.Application.Interfaces;
using MassTransit;

namespace PaymentBackend.Infrastructure.Messaging;

public class RabbitMQMessageBus : IMessageBus
{
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitMQMessageBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}