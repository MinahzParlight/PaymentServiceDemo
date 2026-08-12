using PaymentBackend.Application.DTOs;
using PaymentBackend.Application.Interfaces;
using PaymentBackend.Domain.Interfaces;

namespace PaymentBackend.Application.Features.Payments.Queries.GetPaymentDetails;
public class GetPaymentDetailsQueryHandler
{
    private readonly IPaymentReadRepository _readRepository;
    private readonly ICacheService _cacheService;
    public GetPaymentDetailsQueryHandler(
        IPaymentReadRepository readRepository,
        ICacheService cacheService)
    {
        _readRepository = readRepository;
        _cacheService = cacheService;
    }
    public async Task<PaymentDetailsDto> Handle(GetPaymentDetailsQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = $"PaymentDetails_{query.TransactionId}";
        // Kiểm tra trong Redis trước
        var cachedData = await _cacheService.GetAsync<PaymentDetailsDto>(cacheKey, cancellationToken);
        if (cachedData != null)
        {
            return cachedData; // Cache Hit
        }
        // Nếu Cache Miss, gọi Database qua Read Repository
        // Chú ý: Ở đây ta gọi IPaymentReadRepository thay vì WriteRepository
        var paymentDetails = await _readRepository.GetPaymentDetailsAsync<PaymentDetailsDto>(query.TransactionId);
        if (paymentDetails != null)
        {
            // Lưu vào Redis cho các lần truy vấn sau (Ví dụ: Thiết lập TTL 15 phút)
            await _cacheService.SetAsync(cacheKey, paymentDetails, TimeSpan.FromMinutes(15), cancellationToken);
        }
        return paymentDetails;
    }
}