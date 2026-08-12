
namespace PaymentBackend.Application.Interfaces;

/// <summary>
/// Interface cho Unit of Work, đảm bảo tính toàn vẹn của transaction DB khi Ghi
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}