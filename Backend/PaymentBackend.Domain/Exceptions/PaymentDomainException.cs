namespace PaymentBackend.Domain.Exceptions;
    /// <summary>
    /// Custom Exception dành riêng cho tầng Domain, giúp phân biệt với các lỗi hệ thống (như NullReference, SqlException)
    /// </summary>
public class PaymentDomainException : Exception
{
    public PaymentDomainException() { }
    public PaymentDomainException(string message) : base(message) { }
    public PaymentDomainException(string message, Exception innerException) : base(message, innerException) { }
}