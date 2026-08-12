using PaymentBackend.Domain.Exceptions;

namespace PaymentBackend.Domain.ValueObjects;
    /// <summary>
    /// Value Object: Chứa thông tin tham chiếu lỏng lẻo tới Đơn hàng (Loose Coupling).
    /// Hệ thống Payment không cần biết Đơn hàng có những mặt hàng nào, chỉ cần ID và Số tiền.
    /// </summary>
public class OrderReference
{
    public string OrderId { get; private set; }
    public decimal TotalAmount { get; private set; }

    public OrderReference(string orderId, decimal totalAmount)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new PaymentDomainException("Mã đơn hàng không được để trống.");
        
        if (totalAmount <= 0)
            throw new PaymentDomainException("Số tiền thanh toán phải lớn hơn 0.");

        OrderId = orderId;
        TotalAmount = totalAmount;
    }
}