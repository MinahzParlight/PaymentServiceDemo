using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using StackExchange.Redis;
using PaymentBackend.Application.Interfaces;
using PaymentBackend.Domain.Interfaces;
using PaymentBackend.Infrastructure.Caching;
using PaymentBackend.Infrastructure.Gateways;
using PaymentBackend.Infrastructure.Messaging;
using PaymentBackend.Infrastructure.Persistence;
using PaymentBackend.Infrastructure.Persistence.Repositories;
using PaymentBackend.Infrastructure.Services;

namespace PaymentBackend.Infrastructure
{
    public static class DependencyInjection
    {
        // Hàm này sẽ được gọi từ Program.cs
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký Database (EF Core)
            services.AddDbContext<PaymentDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Repositories & UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPaymentWriteRepository, PaymentWriteRepository>();
            services.AddScoped<IPaymentReadRepository, PaymentReadRepository>();

            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";

            // Đăng ký Caching (Redis)
            services.AddStackExchangeRedisCache(options => {
                options.Configuration = redisConnectionString;
            });
            services.AddSingleton<ICacheService, RedisCacheService>();

            services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(redisConnectionString));

            // Đăng ký Service cung cấp Khóa phân tán cho tầng Application
            services.AddScoped<IDistributedLockService, RedisLockService>();

            // Đăng ký Message Broker
            services.AddScoped<IMessageBus, RabbitMQMessageBus>();

            // Đăng ký Factory & Payment Strategies
            services.AddSingleton<IPaymentStrategyFactory, PaymentStrategyFactory>();
            
            // Đăng ký các implementation để Factory có thể resolve
            services.AddTransient<CreditCardPaymentStrategy>();
            services.AddTransient<EWalletPaymentStrategy>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    // Đọc thông tin kết nối từ appsettings.json
                    cfg.Host(configuration["RabbitMQ:Host"], h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"]);
                        h.Password(configuration["RabbitMQ:Password"]);
                    });
                });
            });

            return services;
        }
    }
}