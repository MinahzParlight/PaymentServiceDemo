using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

using PaymentBackend.Application;
using PaymentBackend.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment System Api",
        Version = "v1",
        Description = "Api xử lý thanh toán"
    });
});


builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PaymentBackend.Application.Features.Payments.Commands.ProcessPayment.ProcessPaymentCommandHandler).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(PaymentBackend.Application.Features.Payments.Commands.ProcessPayment.ProcessPaymentCommandValidator).Assembly);
builder.Services.AddApplicationServices();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    return ConnectionMultiplexer.Connect(configuration);
});


var app = builder.Build();

app.UseMiddleware<PaymentBackend.Api.Middlewares.GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
