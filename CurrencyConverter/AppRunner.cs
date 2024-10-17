using System.Text.Json;
using CurrencyConverter.Domain.Models;
using CurrencyConverter.Infrastructure.Services.CurrencyConversion;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverter;

public class AppRunner
{
    private readonly JsonSerializerOptions _serializationOptions = new()
    {
        WriteIndented = true
    };
    private readonly List<ConsoleKey> _actionApprovalButtons =
        [ConsoleKey.Enter, ConsoleKey.Escape, ConsoleKey.Backspace];

    private int _selectionModeLine = 1;
    private bool _exitRequested;

    public async Task StartAsync(IServiceProvider serviceProvider)
    {
        var lithuanianBankCacheService = serviceProvider.GetRequiredService<ILithuanianBankCacheService>();

        while (true)
        {
            PrintActions();
            if (_exitRequested) break;

            switch (_selectionModeLine)
            {
                case 1:
                {
                    Console.WriteLine("Only currency names?\ty / N");
                    var simplified = Console.ReadKey().Key;
                    while (simplified is not (ConsoleKey.Y or ConsoleKey.N or ConsoleKey.Enter))
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        simplified = Console.ReadKey().Key;
                    }

                    var result = await lithuanianBankCacheService.GetCurrencyListAsync();
                    PrintContent(simplified == ConsoleKey.Y ? result?.CcyNtries.Select(x => x.Ccy) : result);
                    break;
                }
                case 2:
                {
                    PrintContent(await GetFxRatesForDateAsync(lithuanianBankCacheService));
                    break;
                }
                case 3:
                {
                    PrintContent(await GetCurrentFxRatesAsync(lithuanianBankCacheService));
                    break;
                }
                case 4:
                {
                    PerformConvert(await GetFxRatesForDateAsync(lithuanianBankCacheService));
                    break;
                }
                case 5:
                {
                    PerformConvert(await GetCurrentFxRatesAsync(lithuanianBankCacheService));
                    break;
                }
            }

            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
        }

        return;

        void PrintContent(object? content)
        {
            if (content is null)
            {
                Console.WriteLine("Content came in as null. Nothing to print");
                return;
            }

            var stringContent = JsonSerializer.Serialize(content, _serializationOptions);
            Console.WriteLine(stringContent);
        }

        FxRate? FindFxRateMatches(List<FxRate> rates, string from, string to)
        {
            var currencies = new List<string> { from.ToUpper(), to.ToUpper() };
            foreach (var fxRateEntry in rates)
            {
                var matched = 0;
                foreach (var currencyAmount in fxRateEntry.CurrencyAmounts)
                {
                    if (currencies.Contains(currencyAmount.Currency.ToUpper())) ++matched;
                }

                if (matched == 2)
                {
                    return fxRateEntry;
                }
            }

            return null;
        }

        void PerformConvert(FxRates? fxRates)
        {
            if (fxRates is null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to retrieve currency rates");
                Console.ResetColor();
                return;
            }
            
            var currencyFrom = ForceCorrectValue(
                val => !fxRates.FxRateList.Any(x => x.CurrencyAmounts.Any(y => y.Currency == val?.ToUpper())),
                "What currency to convert from? (Default EUR)", "Unable to find this currency", "EUR");

            decimal amountParsed = 0;
            var amount = ForceCorrectValue(
                val => decimal.TryParse(val, out amountParsed),
                "What amount do you want to convert?", "Unable to parse amount, try again");

            var currencyTo = ForceCorrectValue(
                val => !fxRates.FxRateList.Any(x => x.CurrencyAmounts.Any(y => y.Currency == val?.ToUpper())),
                "What currency to convert to?", "Unable to find this currency");

            var fxRate = FindFxRateMatches(fxRates.FxRateList, currencyFrom, currencyTo);
            if (fxRate is null)
            {
                Console.WriteLine("Unable to find a match for these currencies");
                return;
            }

            decimal? converted = 0;
            var ccyAmtFrom = fxRate.CurrencyAmounts.FirstOrDefault(x => x.Currency == currencyFrom);
            if (ccyAmtFrom?.Amount == 1)
            {
                var toAmount = fxRate.CurrencyAmounts.FirstOrDefault(x => x.Currency == currencyTo)?.Amount;
                converted = toAmount * amountParsed;
            }
            else
            {
                converted = amountParsed / ccyAmtFrom?.Amount;
            }

            PrintContent(new
            {
                ConvertedAmount = Math.Round(converted ?? 0, 2, MidpointRounding.AwayFromZero),
                ConvertedFrom = currencyFrom,
                ConvertedTo = currencyTo,
                RateAtDate = fxRate.Date.ToString("yyyy-MM-dd")
            });
        }
    }

    private string ForceCorrectValue<TModel>(Func<string?, TModel?> whileCondition, string dataDescription,
        string retryMessage, string? defaultValue = null)
    {
        Console.WriteLine(dataDescription);
        var data = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(data) && !string.IsNullOrWhiteSpace(defaultValue)) return defaultValue;
        
        var result = whileCondition(data);
        while (result is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(retryMessage);
            Console.ResetColor();

            data = Console.ReadLine();
            result = whileCondition(data);
            if (string.IsNullOrWhiteSpace(data) && !string.IsNullOrWhiteSpace(defaultValue)) return defaultValue;
        }

        return data ?? string.Empty;
    }

    private async Task<FxRates?> GetCurrentFxRatesAsync(ILithuanianBankCacheService lithuanianBankCacheService)
    {
        Console.WriteLine("What is the Course Type? (Default EU)");
        var tp = Console.ReadLine();
        tp = string.IsNullOrWhiteSpace(tp) ? "EU" : tp;

        return await lithuanianBankCacheService.GetCurrentCurrencyValuesAsync(tp);
    }

    private async Task<FxRates?> GetFxRatesForDateAsync(ILithuanianBankCacheService lithuanianBankCacheService)
    {
        var dateOnly = DateOnly.MinValue;
        var date = ForceCorrectValue(
            val => DateOnly.TryParse(val, out dateOnly),
            "Date for which to get the rates? (format YYYY-MM-DD)", "Unable to parse date, try again");

        Console.WriteLine("What is the Course Type? (Default EU)");
        var tp = Console.ReadLine();
        tp = string.IsNullOrWhiteSpace(tp) ? "EU" : tp;

        return await lithuanianBankCacheService.GetCurrencyValuesForDateAsync(dateOnly, tp);
    }

    private void PrintActions()
    {
        Console.Clear();
        Console.WriteLine("Currency converter --");
        _selectionModeLine = 1;
        var options = new Dictionary<int, string>
        {
            { 1, "Get currencies list" },
            { 2, "Get currency for date" },
            { 3, "Get current currencies" },
            { 4, "Convert from A to B currency at specific date currency" },
            { 5, "Convert from A to B currency at current currency" },
            { 0, "Press Esc or backspace to exit" }
        };

        OptionsPrintout();

        var key = Console.ReadKey().Key;
        while (!_actionApprovalButtons.Contains(key))
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    Console.ResetColor();
                    _selectionModeLine = Math.Max(1, --_selectionModeLine);
                    break;
                case ConsoleKey.DownArrow:
                    Console.ResetColor();
                    _selectionModeLine = Math.Min(5, ++_selectionModeLine);
                    break;
                default:
                    key = Console.ReadKey().Key;
                    continue;
            }

            OptionsPrintout();
            key = Console.ReadKey().Key;
        }

        if (key is ConsoleKey.Escape or ConsoleKey.Backspace) _exitRequested = true;

        void OptionsPrintout()
        {
            Console.SetCursorPosition(0, 1);
            foreach (var option in options)
            {
                if (option.Key == _selectionModeLine)
                    Console.ForegroundColor = ConsoleColor.Green;
                else Console.ResetColor();

                if (option.Key > 0)
                    Console.WriteLine("{0}. {1}", option.Key, option.Value);
                else
                    Console.WriteLine(option.Value);
            }
        }
    }
}
