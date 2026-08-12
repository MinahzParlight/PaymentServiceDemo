using PaymentBackend.Domain.Interfaces;
using PaymentBackend.Domain.ValueObjects;

namespace PaymentBackend.Infrastructure.Gateways;

public class CreditCardPaymentStrategy : IPaymentStrategy
{
    public async Task<PaymentResult> ProcessPaymentAsync(OrderReference order)
    {
        // Giả lập gọi API bên ngoài (ví dụ gọi qua Stripe API)
        await Task.Delay(500); // Simulate network latency
        // Trong thực tế sẽ gọi: var response = await _httpClient.PostAsync("stripe_url", data);
        
        // Giả lập thành công ngẫu nhiên
        if (order.TotalAmount > 0)
        {
            var gatewayRefId = "STRIPE_" + Guid.NewGuid().ToString().Substring(0, 8);
            return PaymentResult.Success(gatewayRefId);
        }
        return PaymentResult.Failure("Thẻ không được hỗ trợ hoặc hết hạn.");
    }
}