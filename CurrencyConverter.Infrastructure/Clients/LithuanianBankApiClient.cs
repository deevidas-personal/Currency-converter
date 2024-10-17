using CurrencyConverter.Domain.Helpers;
using CurrencyConverter.Domain.Models;

namespace CurrencyConverter.Infrastructure.Clients;

public class LithuanianBankApiClient(HttpClient client) : ILithuanianBankApiClient
{
    public async Task<CcyTbl?> GetCurrencyListAsync()
    {
        var contentString = await (await client.GetAsync("getCurrencyList")).Content.ReadAsStringAsync();
        return contentString.DeserializeXml<CcyTbl>();
    }

    public async Task<FxRates?> GetCurrencyValuesForDateAsync(DateOnly dateTime, string tp)
    {
        var contentString =
            await (await client
                    .GetAsync($"getFxRates?tp={tp}&dt={dateTime.ToString("yyyy-MM-dd")}"))
                .Content.ReadAsStringAsync();

        return contentString.DeserializeXml<FxRates>();
    }

    public async Task<FxRates?> GetCurrencyCurrentValuesAsync(string tp = "EU")
    {
        var contentString =
            await (await client
                    .GetAsync($"getCurrentFxRates?tp={tp}"))
                .Content.ReadAsStringAsync();

        return contentString.DeserializeXml<FxRates>();
    }
}
