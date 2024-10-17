using CurrencyConverter.Domain.Models;

namespace CurrencyConverter.Infrastructure.Services.CurrencyConversion;

public interface ILithuanianBankCacheService
{
    Task<CcyTbl?> GetCurrencyListAsync();
    Task<FxRates?> GetCurrencyValuesForDateAsync(DateOnly dateTime, string tp);
    Task<FxRates?> GetCurrentCurrencyValuesAsync(string tp);
}
