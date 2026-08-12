using Microsoft.EntityFrameworkCore;
using PaymentBackend.Domain.Entities;

namespace PaymentBackend.Infrastructure.Persistence;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }
    public DbSet<PaymentTransaction> Transactions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Áp dụng các cấu hình Entity riêng biệt thay vì viết hết ở đây
        modelBuilder.ApplyConfiguration(new Configurations.PaymentTransactionConfiguration());
    }
}