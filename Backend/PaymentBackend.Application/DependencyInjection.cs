using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace PaymentBackend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Lấy Assembly hiện tại (chứa toàn bộ code của tầng Application)
        var assembly = Assembly.GetExecutingAssembly();
        // 1. Đăng ký MediatR (Tự động quét và đăng ký tất cả các CommandHandler, QueryHandler)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        // 2. Đăng ký FluentValidation (Tự động quét và đăng ký tất cả các AbstractValidator)
        // Lệnh này lấy từ package: FluentValidation.DependencyInjectionExtensions
        services.AddValidatorsFromAssembly(assembly);
        // 3. (Tùy chọn) Đăng ký các Behaviors (Middleware của MediatR) tại đây
        // Ví dụ: services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}
