using PaymentBackend.Application.Interfaces;

namespace PaymentBackend.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly PaymentDbContext _dbContext;
    public UnitOfWork(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Tự động quản lý transaction. Nếu gọi SaveChanges mà lỗi, EF Core tự động Rollback.
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}