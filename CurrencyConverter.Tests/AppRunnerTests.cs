using CurrencyConverter.Infrastructure.Clients;
using CurrencyConverter.Infrastructure.Services.CurrencyConversion;
using CurrencyConverter.Tests.Mocks;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace CurrencyConverter.Tests;

public class AppRunnerTests
{
    private readonly Mock<ILithuanianBankApiClient> _lithuanianBankApiServiceMock;
    private readonly ILithuanianBankCacheService _lithuanianBankCacheService;
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    
    public AppRunnerTests()
    {
        _memoryCacheMock = new Mock<IMemoryCache>();
        _lithuanianBankApiServiceMock = new Mock<ILithuanianBankApiClient>();
        _lithuanianBankCacheService = new LithuanianBankCacheService(_lithuanianBankApiServiceMock.Object, _memoryCacheMock.Object);
    }
    
    //
    // For integration tests to run there has to be multiple changes made:
    // • There should be a ConsoleWrapper where I would have to create a virtual method where during tests I define the answers of the ReadLine/ReadKey
    // • The methods should be exposed more since currently this app runs until exit is requested and that way we can't see the test results of the functions we're calling
    //
    
    [Fact]
    public async Task GetRates_IMemoryCacheTriggered_InsertOnNotFound()
    {
        var tp = "EU";
        
        _lithuanianBankApiServiceMock.Setup(x => x.GetCurrencyCurrentValuesAsync(tp))
            .ReturnsAsync(FxRatesMocks.FxRates);
        _memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>());

        var result = await _lithuanianBankCacheService.GetCurrentCurrencyValuesAsync(tp);
        
        Assert.NotNull(result);
        Assert.NotEmpty(result.FxRateList);
        _memoryCacheMock.Verify(x => x.CreateEntry(It.IsAny<string>()));
        _memoryCacheMock.Verify(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny!));
    }
    
    [Fact]
    public async Task GetRates_IMemoryCacheTriggered_GetOnFound()
    {
        object mockObjects = FxRatesMocks.FxRates;
        _memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out mockObjects!))
            .Returns(true);
        var result = await _lithuanianBankCacheService.GetCurrentCurrencyValuesAsync("EU");
        
        Assert.NotNull(result);
        Assert.NotEmpty(result.FxRateList);
        _memoryCacheMock.Verify(x => x.TryGetValue(It.IsAny<string>(), out It.Ref<object>.IsAny!));
        _memoryCacheMock.VerifyNoOtherCalls();
    }
}
