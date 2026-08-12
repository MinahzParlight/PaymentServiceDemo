using PaymentBackend.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;

namespace PaymentBackend.Infrastructure.Persistence.Repositories;

public class PaymentReadRepository : IPaymentReadRepository
{
    private readonly string _connectionString;
    // Bơm IConfiguration để lấy chuỗi kết nối trực tiếp
    public PaymentReadRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    public async Task<T> GetPaymentDetailsAsync<T>(string transactionId)
    {
        // Viết SQL thô cực nhanh, lấy vừa đủ dữ liệu, map trực tiếp sang DTO bằng Dapper
        var sql = @"
            SELECT 
                TransactionId, 
                TotalAmount, 
                Status AS StatusName, 
                GatewayReference, 
                FailureReason, 
                CreatedAt 
            FROM PaymentTransactions 
            WHERE TransactionId = @TransactionId";
        using var connection = CreateConnection();
        
        // Dapper tự động map các cột truy vấn được sang properties của class T (PaymentDetailsDto)
        return await connection.QueryFirstOrDefaultAsync<T>(sql, new { TransactionId = transactionId });
    }
    public async Task<IEnumerable<T>> GetHistoricalPaymentsAsync<T>(string orderId, int pageNumber, int pageSize)
    {
        // Sử dụng cú pháp phân trang OFFSET FETCH của SQL Server
        var sql = @"
            SELECT 
                TransactionId, 
                TotalAmount, 
                Status, 
                CreatedAt 
            FROM PaymentTransactions 
            WHERE OrderId = @OrderId
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY";
        var parameters = new 
        { 
            OrderId = orderId, 
            Offset = (pageNumber - 1) * pageSize, 
            PageSize = pageSize 
        };
        using var connection = CreateConnection();
        
        // Trả về thẳng danh sách T (PaymentSummaryDto)
        return await connection.QueryAsync<T>(sql, parameters);
    }
}