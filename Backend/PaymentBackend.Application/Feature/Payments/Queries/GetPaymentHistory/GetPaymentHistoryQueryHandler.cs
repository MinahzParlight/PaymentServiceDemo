using PaymentBackend.Application.DTOs;
using PaymentBackend.Application.Interfaces;
using PaymentBackend.Domain.Interfaces;

namespace PaymentBackend.Application.Features.Payments.Queries.GetHistoricalPayments;

public class GetHistoricalPaymentsQueryHandler
{
    private readonly IPaymentReadRepository _readRepository;
    public GetHistoricalPaymentsQueryHandler(IPaymentReadRepository readRepository)
    {
        _readRepository = readRepository;
    }
    public async Task<IEnumerable<PaymentSummaryDto>> Handle(GetHistoricalPaymentsQuery query, CancellationToken cancellationToken)
    {
        // Query Handler bỏ qua hoàn toàn Entity, gọi thẳng vào ReadRepository (Dapper)
        // để kéo lên danh sách các DTO siêu nhẹ.
        
        var historicalPayments = await _readRepository.GetHistoricalPaymentsAsync<PaymentSummaryDto>(
            query.OrderId, 
            query.PageNumber, 
            query.PageSize);
        return historicalPayments;
    }
}