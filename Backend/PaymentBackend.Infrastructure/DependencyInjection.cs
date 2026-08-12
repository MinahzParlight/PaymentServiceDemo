using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentBackend.Application.Interfaces;
using PaymentBackend.Domain.Interfaces;
using PaymentBackend.Infrastructure.Caching;
using PaymentBackend.Infrastructure.Gateways;
using PaymentBackend.Infrastructure.Messaging;
using PaymentBackend.Infrastructure.Persistence;
using PaymentBackend.Infrastructure.Persistence.Repositories;

namespace PaymentBackend.Infrastructure
{
    public static class DependencyInjection
    {
        // Hàm này sẽ được gọi từ Program.cs
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Đăng ký Database (EF Core)
            services.AddDbContext<PaymentDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 2. Đăng ký Repositories & UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPaymentWriteRepository, PaymentWriteRepository>();
            services.AddScoped<IPaymentReadRepository, PaymentReadRepository>();

            // 3. Đăng ký Caching (Redis)
            services.AddStackExchangeRedisCache(options => {
                options.Configuration = configuration.GetConnectionString("Redis");
            });
            services.AddSingleton<ICacheService, RedisCacheService>();

            // 4. Đăng ký Message Broker
            services.AddSingleton<IMessageBus, RabbitMQMessageBus>();

            // 5. Đăng ký Factory & Payment Strategies
            services.AddSingleton<IPaymentStrategyFactory, PaymentStrategyFactory>();
            
            // Chú ý: Cần đăng ký luôn các implementation để Factory có thể resolve
            services.AddTransient<CreditCardPaymentStrategy>();
            services.AddTransient<EWalletPaymentStrategy>();

            return services;
        }
    }
}