using Microsoft.AspNetCore.Mvc;
using MediatR;

using PaymentBackend.Application.Features.Payments.Commands.ProcessPayment;
using PaymentBackend.Application.Features.Payments.Commands.RefundPayment;
using PaymentBackend.Application.Features.Payments.Queries.GetPaymentDetails;
using PaymentBackend.Application.Features.Payments.Queries.GetHistoricalPayments;

namespace PaymentBackend.Api.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;
    // Tầng API chỉ phụ thuộc duy nhất vào IMediator của thư viện MediatR
    // Không hề biết đến Repository, DbContext hay Entity nào cả!
    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// POST: Tạo mới một giao dịch thanh toán
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand command)
    {
        // Controller không cần gọi Validator. 
        // MediatR Pipeline Behavior (nếu được cấu hình) sẽ tự động chạy Validator trước.
        var transactionId = await _mediator.Send(command);
        
        // Trả về mã 201 Created và kèm theo URL để client gọi GET chi tiết
        return CreatedAtAction(nameof(GetPaymentDetails), new { id = transactionId }, new { TransactionId = transactionId });
    }
    /// <summary>
    /// GET: Lấy chi tiết một giao dịch
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentDetails(string id)
    {
        var query = new GetPaymentDetailsQuery { TransactionId = id };
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
    /// <summary>
    /// POST: Yêu cầu hoàn tiền một giao dịch
    /// </summary>
    [HttpPost("refund")]
    public async Task<IActionResult> RefundPayment([FromBody] RefundPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { Message = result });
    }
    /// <summary>
    /// GET: Lấy danh sách lịch sử giao dịch (có phân trang)
    /// </summary>
    [HttpGet("history/{orderId}")]
    public async Task<IActionResult> GetHistoricalPayments(string orderId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetHistoricalPaymentsQuery 
        { 
            OrderId = orderId, 
            PageNumber = pageNumber, 
            PageSize = pageSize 
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}