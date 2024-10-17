using CurrencyConverter.Domain.Models;
using CurrencyConverter.Infrastructure.Clients;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyConverter.Infrastructure.Services.CurrencyConversion;

public class LithuanianBankCacheService(ILithuanianBankApiClient client, IMemoryCache memoryCache) : ILithuanianBankCacheService
{
    public async Task<CcyTbl?> GetCurrencyListAsync()
    {
        const string cacheKey = "currencyList";
        return await GetOrSetCache(client.GetCurrencyListAsync, cacheKey);
    }

    public async Task<FxRates?> GetCurrencyValuesForDateAsync(DateOnly dateTime, string tp)
    {
        var cacheKey = "currencyList-" + dateTime.ToString("yyyy-MM-dd");
        return await GetOrSetCache(() => client.GetCurrencyValuesForDateAsync(dateTime, tp), cacheKey);
    }

    public async Task<FxRates?> GetCurrentCurrencyValuesAsync(string tp)
    {
        var cacheKey = "currencyList-current";
        return await GetOrSetCache(() => client.GetCurrencyCurrentValuesAsync(tp), cacheKey);
    }

    private async Task<TModel?> GetOrSetCache<TModel>(Func<Task<TModel?>> action, string cacheKey) where TModel : class
    {
        if (memoryCache.TryGetValue(cacheKey, out var currencyList)) return currencyList as TModel;

        var result = await action();
        if (result is not null) memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

        return result;
    }
}
