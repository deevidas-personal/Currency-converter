using CurrencyConverter.Domain.Models;

namespace CurrencyConverter.Infrastructure.Clients;

public interface ILithuanianBankApiClient
{
    Task<CcyTbl?> GetCurrencyListAsync();
    Task<FxRates?> GetCurrencyValuesForDateAsync(DateOnly dateTime, string tp);
    Task<FxRates?> GetCurrencyCurrentValuesAsync(string tp = "EU");
}
