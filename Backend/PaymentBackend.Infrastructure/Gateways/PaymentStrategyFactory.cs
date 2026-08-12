using Microsoft.Extensions.DependencyInjection;

using PaymentBackend.Application.Interfaces;
using PaymentBackend.Domain.Interfaces;

namespace PaymentBackend.Infrastructure.Gateways;

public class PaymentStrategyFactory : IPaymentStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        // Dùng IServiceProvider để lấy các strategy đã được inject qua DI (Tránh tạo cứng đối tượng bằng từ khóa 'new')
        public PaymentStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentStrategy GetStrategy(string paymentType)
        {
            return paymentType.ToUpper() switch
            {
                "CREDIT_CARD" => _serviceProvider.GetRequiredService<CreditCardPaymentStrategy>(),
                "E_WALLET" => _serviceProvider.GetRequiredService<EWalletPaymentStrategy>(),
                _ => throw new ArgumentException($"Không hỗ trợ phương thức thanh toán: {paymentType}")
            };
        }

        public bool IsSupported(string paymentType)
        {
            return paymentType.ToUpper() switch
            {
                "CREDIT_CARD" => true,
                "E_WALLET" => true,
                _ => false
            };
        }
    }