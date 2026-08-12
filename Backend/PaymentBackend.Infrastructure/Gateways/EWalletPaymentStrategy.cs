using PaymentBackend.Domain.Interfaces;
using PaymentBackend.Domain.ValueObjects;

namespace PaymentBackend.Infrastructure.Gateways;

public class EWalletPaymentStrategy : IPaymentStrategy
{
    public async Task<PaymentResult> ProcessPaymentAsync(OrderReference order)
    {
        // Giả lập gọi API bên ngoài (ví dụ gọi qua Momo API)
        await Task.Delay(300);
        // Giả lập từ chối nếu số tiền quá lớn (Lỗi nghiệp vụ từ đối tác)
        if (order.TotalAmount > 20000000) // Lớn hơn 20tr
        {
            return PaymentResult.Failure("Ví điện tử vượt quá hạn mức thanh toán trong ngày.");
        }
        var gatewayRefId = "MOMO_" + Guid.NewGuid().ToString().Substring(0, 8);
        return PaymentResult.Success(gatewayRefId);
    }
}