using FluentValidation;
using PaymentBackend.Application.Interfaces;

namespace PaymentBackend.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    private readonly IPaymentStrategyFactory _strategyFactory;
    public ProcessPaymentCommandValidator(IPaymentStrategyFactory strategyFactory)
    {
        _strategyFactory = strategyFactory;

        RuleFor(v => v.OrderId)
            .NotEmpty().WithMessage("Mã đơn hàng không được để trống.");
        RuleFor(v => v.Amount)
            .GreaterThan(0).WithMessage("Số tiền thanh toán phải lớn hơn 0.");
        RuleFor(v => v.PaymentType)
                .NotEmpty().WithMessage("Phương thức thanh toán không được để trống.")
                .Must(BeAValidPaymentType).WithMessage("Phương thức thanh toán không hợp lệ hoặc không được hỗ trợ.");
    }
    private bool BeAValidPaymentType(string paymentType)
    {
        // Ủy quyền cho Factory để tuân thủ OCP
            return _strategyFactory.IsSupported(paymentType);
    }
}