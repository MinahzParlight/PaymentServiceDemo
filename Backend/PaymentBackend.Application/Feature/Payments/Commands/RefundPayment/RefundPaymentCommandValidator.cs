using FluentValidation;

namespace PaymentBackend.Application.Features.Payments.Commands.RefundPayment;
public class RefundPaymentCommandValidator : AbstractValidator<RefundPaymentCommand>
{
    public RefundPaymentCommandValidator()
    {
        RuleFor(v => v.TransactionId)
            .NotEmpty().WithMessage("Mã giao dịch gốc không được để trống.");
        RuleFor(v => v.RefundAmount)
            .GreaterThan(0).WithMessage("Số tiền hoàn phải lớn hơn 0.");
        RuleFor(v => v.Reason)
            .NotEmpty().WithMessage("Lý do hoàn tiền không được để trống.")
            .MaximumLength(255).WithMessage("Lý do không được vượt quá 255 ký tự.");
    }
}