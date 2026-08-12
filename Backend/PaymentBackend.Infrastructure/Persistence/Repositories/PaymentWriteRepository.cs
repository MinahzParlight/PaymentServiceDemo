using PaymentBackend.Domain.Entities;
using PaymentBackend.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PaymentBackend.Infrastructure.Persistence.Repositories;

public class PaymentWriteRepository : IPaymentWriteRepository
{
    private readonly PaymentDbContext _dbContext;
    public PaymentWriteRepository(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(PaymentTransaction transaction)
    {
        await _dbContext.Transactions.AddAsync(transaction);
        // Không gọi SaveChanges ở đây, nhường việc đó cho Unit of Work
    }
    public async Task<PaymentTransaction> GetByIdAsync(string transactionId)
    {
        // Mặc định DbContext trả về Entity có tracking.
        // Điều này rất quan trọng để khi gọi transaction.Execute(), EF Core nhận ra có sự thay đổi State.
        return await _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
    }
}