using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentBackend.Domain.Entities;

namespace PaymentBackend.Infrastructure.Persistence.Configurations;
public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");
        builder.HasKey(t => t.TransactionId);
        // Cấu hình độ dài cột
        builder.Property(t => t.TransactionId).HasMaxLength(50).IsRequired();
        builder.Property(t => t.GatewayReference).HasMaxLength(100);
        builder.Property(t => t.FailureReason).HasMaxLength(255);
        
        // Map Enum ra chuỗi (string) trong DB để dễ đọc (VD: lưu 'Success' thay vì số 2)
        builder.Property(t => t.Status)
               .HasConversion<string>()
               .HasMaxLength(20);
        // Kỹ thuật mapping Value Object: OrderReference
        // EF Core sẽ gộp các thuộc tính của OrderReference vào cùng bảng PaymentTransactions (OwnsOne)
        builder.OwnsOne(t => t.Order, order =>
        {
            order.Property(o => o.OrderId).HasColumnName("OrderId").HasMaxLength(50).IsRequired();
            order.Property(o => o.TotalAmount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)").IsRequired();
        });
    }
}