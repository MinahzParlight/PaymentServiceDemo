namespace PaymentBackend.Domain.ValueObjects;
    /// <summary>
    /// Value Object: Trả về kết quả từ IPaymentStrategy sau khi gọi cổng thanh toán
    /// </summary>
public class PaymentResult
{
    public bool IsSuccess { get; private set; }
    public string? TransactionReference { get; private set; } // Mã GD từ đối tác
    public string? ErrorMessage { get; private set; }

    private PaymentResult(bool isSuccess, string? transactionReference, string? errorMessage)
    {
        IsSuccess = isSuccess;
        TransactionReference = transactionReference;
        ErrorMessage = errorMessage;
    }
        

    public static PaymentResult Success(string transactionReference) 
        => new PaymentResult(true, transactionReference, null);

    public static PaymentResult Failure(string errorMessage) 
        => new PaymentResult(false, null, errorMessage);
}
