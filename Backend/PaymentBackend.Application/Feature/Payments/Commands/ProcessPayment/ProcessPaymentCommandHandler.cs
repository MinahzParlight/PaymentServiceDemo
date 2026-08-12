using PaymentBackend.Domain.Interfaces;
using PaymentBackend.Application.Interfaces;
using PaymentBackend.Domain.ValueObjects;
using PaymentBackend.Domain.Entities;

namespace PaymentBackend.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler // : IRequestHandler<ProcessPaymentCommand, string>
{
    private readonly IPaymentWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _messageBus;
    private readonly IPaymentStrategyFactory _strategyFactory; 
    public ProcessPaymentCommandHandler(
        IPaymentWriteRepository writeRepository,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus,
        IPaymentStrategyFactory strategyFactory)
    {
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
        _strategyFactory = strategyFactory;
    }
    public async Task<string> Handle(ProcessPaymentCommand command, CancellationToken cancellationToken)
    {
        // Chuẩn bị Value Object (Tham chiếu lỏng lẻo tới Order - Bounded Context)
        var orderRef = new OrderReference(command.OrderId, command.Amount);
        // Lấy đúng chiến lược thanh toán (Strategy Pattern)
        var paymentStrategy = _strategyFactory.GetStrategy(command.PaymentType);
        // Khởi tạo Domain Entity
        var transaction = new PaymentTransaction(orderRef);
        // Ủy quyền cho Entity TỰ XỬ LÝ logic cốt lõi của nó (Rich Domain Model)
        // Lưu ý: Các lỗi nghiệp vụ (như số dư không đủ) sẽ ném ra Exception từ tầng Domain
        await transaction.ExecuteAsync(paymentStrategy);
        // Thêm vào Repository (Lưu ý: Lúc này chưa lưu vào DB thực tế)
        await _writeRepository.AddAsync(transaction);
        // Commit dữ liệu xuống Database (Save Changes) qua Unit of Work
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Bắn Event ra Message Bus (RabbitMQ) để các hệ thống khác tự động đồng bộ (Event-Driven)
        if (transaction.Status == Domain.Enums.TransactionStatus.Success)
        {
            var successEvent = new { transaction.TransactionId, command.OrderId, command.Amount };
            await _messageBus.PublishAsync(successEvent, cancellationToken);
        }
        // Trả về Id giao dịch ra ngoài Controller
        return transaction.TransactionId;
    }
}