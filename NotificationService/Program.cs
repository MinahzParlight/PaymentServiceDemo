using MassTransit;
using NotificationService.Consumers;

var builder = Host.CreateApplicationBuilder(args);

// Cấu hình MassTransit kết nối RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // 1. Đăng ký cái Consumer vừa viết
    x.AddConsumer<EmailNotificationConsumer>();

    // 2. Trỏ tới RabbitMQ đang chạy trên Docker của bạn
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // 3. Khai báo cái Hàng đợi (Queue) tên là "email-notification-queue"
        cfg.ReceiveEndpoint("email-notification-queue", e =>
        {
            e.ConfigureConsumer<EmailNotificationConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();