namespace PaymentBackend.Domain.Enums;
    /// <summary>
    /// Vòng đời trạng thái của một giao dịch thanh toán
    /// </summary>
public enum TransactionStatus
{
    Created = 0,
    Processing = 1,
    Success = 2,
    Failed = 3,
    Refunded = 4
}