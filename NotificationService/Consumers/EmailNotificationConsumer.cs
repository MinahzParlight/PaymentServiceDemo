using MassTransit;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

using PaymentBackend.Domain.Events;

namespace NotificationService.Consumers;

public class EmailNotificationConsumer : IConsumer<PaymentSucceededEvent>
{
    private readonly ILogger<EmailNotificationConsumer> _logger;
    private readonly IConfiguration _configuration;

    public EmailNotificationConsumer(ILogger<EmailNotificationConsumer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var data = context.Message;
        string receiverEmail = "pause.delete.hai@gmail.com";
        _logger.LogInformation($"[BẮT ĐẦU] Tiến hành gửi mail");
        
        var senderEmail = _configuration["EmailConfig:SenderEmail"];
        var appPassword = _configuration["EmailConfig:AppPassword"];

        if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(appPassword))
        {
            _logger.LogError("LỖI: Chưa cấu hình Email gửi hoặc Mật khẩu trong User Secrets!");
            return;
        }

        try
        {
            // 1. TẠO NỘI DUNG EMAIL
            var emailMessage = new MimeMessage();
            
            emailMessage.From.Add(new MailboxAddress("Hệ thống Thanh Toán", senderEmail));
            
            // Gửi tới email nhận được từ RabbitMQ
            emailMessage.To.Add(new MailboxAddress("Khách hàng", receiverEmail));
            
            emailMessage.Subject = $"Xác nhận thanh toán thành công đơn hàng {data.OrderId}";

            // Thiết kế giao diện nội dung mail bằng HTML
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <h2 style='color: #2e6c80;'>GIAO DỊCH THÀNH CÔNG!</h2>
                    <ul style='font-size: 16px; line-height: 1.6;'>
                        <li><strong>Mã GD:</strong> {data.TransactionId}</li>
                        <li><strong>Mã đơn hàng:</strong> {data.OrderId}</li>
                        <li><strong>Số tiền:</strong> <strong style='color: green;'>{data.Amount:N0} VNĐ</strong></li>
                        <li><strong>Thời gian:</strong> {data.OccurredOn:dd/MM/yyyy HH:mm:ss}</li>
                    </ul>
                "
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // 2. KẾT NỐI VÀ GỬI QUA SMTP GMAIL
            using var smtp = new SmtpClient();
            
            // Bỏ qua chứng chỉ SSL nếu chạy localhost (tùy chọn để tránh lỗi chứng chỉ)
            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            // Gmail dùng cổng 587 và mã hóa STARTTLS
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            // QUAN TRỌNG: THAY EMAIL VÀ MẬT KHẨU ỨNG DỤNG (16 KÝ TỰ) CỦA BẠN VÀO ĐÂY
            // Lưu ý: Mật khẩu 16 ký tự viết liền, không có dấu cách
            await smtp.AuthenticateAsync(senderEmail, appPassword);

            // Gửi thư
            await smtp.SendAsync(emailMessage);

            // Ngắt kết nối
            await smtp.DisconnectAsync(true);

            _logger.LogInformation($"[THÀNH CÔNG] Đã gửi mail hóa đơn");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[THẤT BẠI] Lỗi khi gửi mail: {ex.Message}");
            // Ném lỗi ra ngoài để MassTransit biết và tự động thử lại (Retry)
            throw; 
        }
    }
}