using PaymentBackend.Domain.ValueObjects;
using PaymentBackend.Domain.Entities;
using System.Threading.Tasks;

namespace PaymentBackend.Domain.Interfaces;

/// <summary>
/// CQRS - Lệnh ĐỌC: Dùng cho Query Handler (Không tương tác với Entity, thường trả về DTO ở tầng Application, 
/// nhưng định nghĩa Interface tại đây để đảo ngược phụ thuộc).
/// </summary>
public interface IPaymentReadRepository
{
    // Sử dụng Generic <T> để tầng Domain không bị phụ thuộc ngược ra ngoài tầng Application (DTO)
    Task<T> GetPaymentDetailsAsync<T>(string transactionId); 
    Task<IEnumerable<T>> GetHistoricalPaymentsAsync<T>(string orderId, int pageNumber, int pageSize); 
}