using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using PaymentBackend.Application;
using PaymentBackend.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PaymentBackend.Application.Features.Payments.Commands.ProcessPayment.ProcessPaymentCommandHandler).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(PaymentBackend.Application.Features.Payments.Commands.ProcessPayment.ProcessPaymentCommandValidator).Assembly);
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<PaymentBackend.Api.Middlewares.GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
