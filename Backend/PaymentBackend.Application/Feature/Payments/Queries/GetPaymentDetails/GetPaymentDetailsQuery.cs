using PaymentBackend.Application.DTOs;
using PaymentBackend.Application.Interfaces;

namespace PaymentBackend.Application.Features.Payments.Queries.GetPaymentDetails;

// (MediatR) QUERY: Yêu cầu lấy dữ liệu (Không làm thay đổi hệ thống)
public class GetPaymentDetailsQuery // : IRequest<PaymentDetailsDto>
{
    public string TransactionId { get; set; } = string.Empty;
}