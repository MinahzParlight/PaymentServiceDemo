namespace PaymentBackend.Application.Interfaces;

/// <summary>
/// Interface cho Caching (Dự trù cho Redis để tối ưu luồng Đọc)
/// </summary>
public interface ICacheService
{
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T data, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default);
}