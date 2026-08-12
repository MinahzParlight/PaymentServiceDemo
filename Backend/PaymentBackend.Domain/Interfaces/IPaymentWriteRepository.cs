using PaymentBackend.Domain.ValueObjects;
using PaymentBackend.Domain.Entities;
using System.Threading.Tasks;

namespace PaymentBackend.Domain.Interfaces;

/// <summary>
/// CQRS - Lệnh GHI: Dùng cho Command Handler để lưu thay đổi trạng thái
/// </summary>
public interface IPaymentWriteRepository
{
    Task AddAsync(PaymentTransaction transaction);
    Task<PaymentTransaction> GetByIdAsync(string transactionId);
}