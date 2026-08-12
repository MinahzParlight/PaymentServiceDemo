using PaymentBackend.Application.Interfaces;

namespace PaymentBackend.Infrastructure.Messaging;

public class RabbitMQMessageBus : IMessageBus
{
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class
    {
        // Dùng các thư viện như MassTransit, hoặc RabbitMQ.Client để gửi message
        
        // var json = JsonSerializer.Serialize(@event);
        // var body = Encoding.UTF8.GetBytes(json);
        // channel.BasicPublish(exchange: "payment_exchange", routingKey: "", basicProperties: null, body: body);
        // Mô phỏng giả lập gửi đi
        Console.WriteLine($"[RabbitMQ] Bắn sự kiện: {@event.GetType().Name} ra Exchange thành công.");
        await Task.CompletedTask;
    }
}