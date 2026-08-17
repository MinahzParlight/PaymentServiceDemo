using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using FluentValidation;

using PaymentBackend.Domain.Exceptions;

namespace PaymentBackend.Api.Middlewares;

/// <summary>
/// Middleware gom tất cả các Exception bị văng ra trong hệ thống và chuyển nó thành mã HTTP chuẩn.
/// Giúp Controller không bao giờ phải viết try-catch rườm rà.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Cho phép request đi tiếp vào Controller
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi: ");
            await HandleExceptionAsync(context, ex);
        }
    }
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        object response = new { error = exception.Message };
        // Phân loại Lỗi để trả về HTTP Status Code cho đúng chuẩn RESTful
        switch (exception)
        {
            case ValidationException validationEx: // Lỗi từ FluentValidation (Dữ liệu đầu vào sai)
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                response = new { error = "Dữ liệu không hợp lệ", details = validationEx.Errors };
                break;
            case PaymentDomainException domainEx: // Lỗi nghiệp vụ từ tầng Domain (VD: Không đủ tiền)
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity; // 422
                response = new { error = domainEx.Message };
                break;
            case KeyNotFoundException notFoundEx: // Lỗi không tìm thấy dữ liệu khi Get
                context.Response.StatusCode = (int)HttpStatusCode.NotFound; // 404
                response = new { error = notFoundEx.Message };
                break;
            default: // Lỗi hệ thống không lường trước (VD: Lỗi kết nối Database, NullReference)
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                response = new { error = "Đã xảy ra lỗi hệ thống nghiêm trọng. Vui lòng thử lại sau." };
                // Trong thực tế, bạn nên ghi log chi tiết lỗi ở đây (Serilog, NLog)
                break;
        }
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}