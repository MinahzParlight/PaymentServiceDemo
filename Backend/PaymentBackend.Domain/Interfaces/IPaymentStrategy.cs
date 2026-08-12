using PaymentBackend.Domain.ValueObjects;
using PaymentBackend.Domain.Entities;
using System.Threading.Tasks;

namespace PaymentBackend.Domain.Interfaces;

/// <summary>
/// Strategy Pattern: Hợp đồng xử lý thanh toán mà các cổng thanh toán phải tuân thủ
/// </summary>
public interface IPaymentStrategy
{
    // Thực thi việc thanh toán ra bên ngoài (gọi API đối tác)
    Task<PaymentResult> ProcessPaymentAsync(OrderReference order);
}