using PaymentBackend.Application.Interfaces;
using PaymentBackend.Domain.Interfaces;
namespace PaymentBackend.Application.Features.Payments.Commands.RefundPayment;

public class RefundPaymentCommandHandler
{
    private readonly IPaymentWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageBus _messageBus;
    public RefundPaymentCommandHandler(
        IPaymentWriteRepository writeRepository,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus)
    {
        _writeRepository = writeRepository;
        _unitOfWork = unitOfWork;
        _messageBus = messageBus;
    }
    public async Task<string> Handle(RefundPaymentCommand command, CancellationToken cancellationToken)
    {
        // Lấy giao dịch gốc từ Database lên
        var transaction = await _writeRepository.GetByIdAsync(command.TransactionId);
        
        if (transaction == null)
        {
            throw new System.Exception("Không tìm thấy giao dịch."); // Trong thực tế nên dùng Domain Exception
        }
        // Kích hoạt logic nghiệp vụ (Rich Domain Model)
        // Lưu ý: Entity PaymentTransaction sẽ cần thêm hàm Refund() để tự kiểm tra logic 
        // (VD: Có đang ở trạng thái Success không? Số tiền hoàn có lố không?)
        // transaction.Refund(command.RefundAmount, command.Reason);
        // Tương tác với API Cổng thanh toán (Thường sẽ tiêm thêm 1 IRefundService vào đây để gọi ra ngoài)
        // bool isRefundSuccess = await _refundService.ProcessRefundAsync(transaction.GatewayReference, command.RefundAmount);
        
        // Nếu thành công, có thể transaction.MarkAsRefunded();
        // Lưu thay đổi xuống Database
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        // Bắn sự kiện ra Message Bus cho hệ thống Kế Toán / Kho
        var refundEvent = new { command.TransactionId, command.RefundAmount, command.Reason };
        await _messageBus.PublishAsync(refundEvent, cancellationToken);
        return "Refund processing initiated successfully.";
    }
}