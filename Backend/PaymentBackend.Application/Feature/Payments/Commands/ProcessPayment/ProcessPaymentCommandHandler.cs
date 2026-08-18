using PaymentBackend.Domain.Interfaces;
using PaymentBackend.Application.Interfaces;
using PaymentBackend.Domain.ValueObjects;
using PaymentBackend.Domain.Entities;
using PaymentBackend.Domain.Events;
using StackExchange.Redis;
using MediatR;

namespace PaymentBackend.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, string>
{
    private readonly IPaymentWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _messageBus;
    private readonly IPaymentStrategyFactory _strategyFactory; 
    private readonly IDistributedLockService _lockService;
    public ProcessPaymentCommandHandler(
        IPaymentWriteRepository writeRepository,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus,
        IPaymentStrategyFactory strategyFactory,
        IDistributedLockService lockService)
    {
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
        _strategyFactory = strategyFactory;
        _lockService = lockService;
    }
    public async Task<string> Handle(ProcessPaymentCommand command, CancellationToken cancellationToken)
    {
        //Thiết lập khóa, hiện tại đặt là OrderId nên có thể xảy ra tình trạng đăng nhập ở 2 nơi và thanh toán
        // Trong trg hợp này, khóa có thể ngăn việc thanh toán cho cùng 1 đơn và trừ tiền 2 lần, 
        // nhưng ko thể giải quyết thanh toán cho 2 đơn khác nhau nhưng số dư ko đủ, hệ thống sẽ cho cả 2 gd hợp lệ
        // Đề xuất thay bằng UserId để khóa cả tài khoản
        
        string lockKey = $"payment_lock_{command.OrderId}";
        string lockToken = Guid.NewGuid().ToString();
        TimeSpan lockExpiry = TimeSpan.FromSeconds(10);

        // Thử lấy khóa
        bool isLocked = await _lockService.AcquireLockAsync(lockKey, lockToken, lockExpiry);
        if (!isLocked)
        {
            throw new Exception("Giao dịch đang được xử lý.");
        }
        try
        {
            // Chuẩn bị Value Object 
            var orderRef = new OrderReference(command.OrderId, command.Amount);
            // Lấy đúng chiến lược thanh toán 
            var paymentStrategy = _strategyFactory.GetStrategy(command.PaymentType);
            // Khởi tạo Domain Entity
            var transaction = new PaymentTransaction(orderRef);
            // Ủy quyền cho Entity TỰ XỬ LÝ logic cốt lõi của nó (Rich Domain Model)
            // Các lỗi nghiệp vụ sẽ ném ra Exception từ tầng Domain
            await transaction.ExecuteAsync(paymentStrategy);
            // Thêm vào Repository (Lúc này chưa lưu vào DB thực tế)
            await _writeRepository.AddAsync(transaction);
            // Commit dữ liệu xuống Database qua Unit of Work
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // Bắn Event ra Message Bus (RabbitMQ) để các hệ thống khác tự động đồng bộ
            if (transaction.Status == Domain.Enums.TransactionStatus.Success)
            {
                var successEvent = new PaymentSucceededEvent(
                    Guid.Parse(transaction.TransactionId), 
                    Guid.Parse(command.OrderId), 
                    command.Amount);
                
                await _messageBus.PublishAsync(successEvent, cancellationToken);
            }
            else if (transaction.Status == Domain.Enums.TransactionStatus.Failed)
            {
                var failEvent = new PaymentFailedEvent(
                    Guid.Parse(transaction.TransactionId), 
                    Guid.Parse(command.OrderId), 
                    command.Amount,
                    transaction.FailureReason
                );

                await _messageBus.PublishAsync(failEvent, cancellationToken);
            }
            // Trả về Id giao dịch ra ngoài Controller
            return transaction.TransactionId;
        }
        finally
        {
            // Nhả khóa
            await _lockService.ReleaseLockAsync(lockKey, lockToken);
        }
    }
}