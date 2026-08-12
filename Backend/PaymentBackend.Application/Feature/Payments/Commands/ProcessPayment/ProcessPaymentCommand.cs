using PaymentBackend.Application.Interfaces;

namespace PaymentBackend.Application.Features.Payments.Commands.ProcessPayment;

// (MediatR) COMMAND: Lệnh yêu cầu thay đổi trạng thái (Mang dữ liệu từ Controller vào)
// Thay vì tạo PaymentRequestDto, ta dùng luôn Command này làm Input model
public class ProcessPaymentCommand // : IRequest<string> (Nếu dùng MediatR)
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentType { get; set; } = string.Empty;
}