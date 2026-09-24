using Booking.Application.Interfaces.Services;

using StackExchange.Redis;
using System.Text.Json;

namespace Booking.Infrastructure.Services;

public class RedisCachingService : ICachingService
{
    private readonly IDatabase _database;
    public RedisCachingService(IConnectionMultiplexer multiplexer)
    {
        _database = multiplexer.GetDatabase();
    }
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default(T?);
        }
        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? exp)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, (exp == null) ? TimeSpan.FromMinutes(15) : exp.Value);

    }

}
