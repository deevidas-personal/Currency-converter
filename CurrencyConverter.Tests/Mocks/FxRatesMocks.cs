using CurrencyConverter.Domain.Models;

namespace CurrencyConverter.Tests.Mocks;

public static class FxRatesMocks
{
    public static FxRates FxRates = new()
    {
        FxRateList =
        [
            new FxRate
            {
                Date = DateTime.Today,
                Type = "EU",
                CurrencyAmounts =
                [
                    new CcyAmt { Amount = 1, Currency = "EUR" },
                    new CcyAmt { Amount = new decimal(1.5), Currency = "USD" }
                ]
            },

            new FxRate
            {
                Date = DateTime.Today,
                Type = "EU",
                CurrencyAmounts =
                [
                    new CcyAmt { Amount = 1, Currency = "EUR" },
                    new CcyAmt { Amount = new decimal(4), Currency = "PLN" }
                ]
            }

        ]
    };
}
