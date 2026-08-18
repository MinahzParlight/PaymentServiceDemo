namespace PaymentBackend.Application.Interfaces;

public interface IDistributedLockService
{
    Task<bool> AcquireLockAsync(string resourceKey, string token, TimeSpan expiry);
    Task<bool> ReleaseLockAsync(string resourceKey, string token);
}