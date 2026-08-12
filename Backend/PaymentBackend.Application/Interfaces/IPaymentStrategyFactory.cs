using PaymentBackend.Domain.Interfaces;

namespace PaymentBackend.Application.Interfaces;

/// <summary>
/// Abstract Factory: Dùng để khởi tạo đúng PaymentStrategy dựa trên Request của người dùng
/// </summary>
public interface IPaymentStrategyFactory 
{
    IPaymentStrategy GetStrategy(string paymentType);
    bool IsSupported(string paymentType);
}