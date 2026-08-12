using PaymentBackend.Domain.Enums;
using PaymentBackend.Domain.ValueObjects;
using PaymentBackend.Domain.Exceptions;
using PaymentBackend.Domain.Interfaces;
using System.Threading.Tasks;

namespace PaymentBackend.Domain.Entities;

/// <summary>
/// Aggregate Root: Trái tim của hệ thống thanh toán. Mọi thay đổi dữ liệu phải đi qua đây.
/// Tính đóng gói (Encapsulation) được áp dụng triệt để qua 'private set'
/// </summary>
public class PaymentTransaction
{
    public string TransactionId { get; private set; } = string.Empty;
    public OrderReference Order { get; private set; } = default!;
    public TransactionStatus Status { get; private set; }
    public string GatewayReference { get; private set; } = string.Empty;
    public string FailureReason { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    // EF Core cần constructor rỗng
    protected PaymentTransaction() { }
    /// <summary>
    /// Khởi tạo một giao dịch mới
    /// </summary>
    public PaymentTransaction(OrderReference order)
    {
        TransactionId = Guid.NewGuid().ToString();
        Order = order ?? throw new PaymentDomainException("Thông tin đơn hàng không hợp lệ.");
        Status = TransactionStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }
    /// <summary>
    /// Hành vi cốt lõi: Thực thi giao dịch với một phương thức thanh toán cụ thể
    /// </summary>
    public async Task ExecuteAsync(IPaymentStrategy paymentStrategy)
    {
        if (Status != TransactionStatus.Created && Status != TransactionStatus.Failed)
            throw new PaymentDomainException("Chỉ có thể thực thi giao dịch ở trạng thái Khởi tạo hoặc Thất bại.");
        if (paymentStrategy == null)
            throw new PaymentDomainException("Không tìm thấy phương thức thanh toán phù hợp.");
        // Chuyển trạng thái sang đang xử lý
        Status = TransactionStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
        // Kích hoạt tính Đa hình thông qua Strategy
        var result = await paymentStrategy.ProcessPaymentAsync(this.Order);
        // Xử lý kết quả trả về từ cổng thanh toán
        if (result.IsSuccess)
        {
            var gatewayReference = result.TransactionReference ?? throw new PaymentDomainException("Gateway reference missing for successful payment.");
            MarkAsSuccess(gatewayReference);
        }
        else
        {
            var failureReason = result.ErrorMessage ?? "Unknown payment failure.";
            MarkAsFailed(failureReason);
        }
    }
    private void MarkAsSuccess(string gatewayReference)
    {
        Status = TransactionStatus.Success;
        GatewayReference = gatewayReference;
        UpdatedAt = DateTime.UtcNow;
        
        // Note: Tại đây trong kiến trúc thực tế có thể sinh ra Domain Event 
        // ví dụ: AddDomainEvent(new PaymentSucceededEvent(this.TransactionId));
    }
    private void MarkAsFailed(string reason)
    {
        Status = TransactionStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
        
        // Note: Sinh ra Domain Event cho việc thanh toán thất bại
    }
}