using CurrencyConverter;
using CurrencyConverter.Domain.Configurations;
using CurrencyConverter.Infrastructure.Clients;
using CurrencyConverter.Infrastructure.Services.CurrencyConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var config = configuration.Build();

var serviceCollection = new ServiceCollection();
serviceCollection.Configure<LithuanianBankConfig>(config.GetSection(nameof(LithuanianBankConfig)).Bind);
serviceCollection.AddMemoryCache();
serviceCollection.AddHttpClient<ILithuanianBankApiClient, LithuanianBankApiClient>((sp, ctx) =>
{
    var ltuBankConfig = sp.GetRequiredService<IOptions<LithuanianBankConfig>>().Value; 
    ctx.BaseAddress = new Uri(ltuBankConfig.BaseUri);
    ctx.DefaultRequestHeaders.Add("Accept", ltuBankConfig.AcceptHeaderDefault);
});
serviceCollection
    .AddTransient<ILithuanianBankCacheService, LithuanianBankCacheService>();

await new AppRunner().StartAsync(serviceCollection.BuildServiceProvider());
