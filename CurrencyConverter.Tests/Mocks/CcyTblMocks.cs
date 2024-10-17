using CurrencyConverter.Domain.Models;

namespace CurrencyConverter.Tests.Mocks;

public static class CcyTblMocks
{
    public static CcyTbl CcyTbls =
        new CcyTbl
        {
            CcyNtries =
            [
                new CcyNtry
                {
                    Ccy = "EUR",
                    CcyNbr = "1",
                    CcyMnrUnts = 1,
                    CcyNames =
                    [
                        new CcyNm { Lang = "EN", Name = "Euro" },
                        new CcyNm { Lang = "LT", Name = "Euras" }
                    ]
                },
                new CcyNtry
                {
                    Ccy = "USD",
                    CcyNbr = "2",
                    CcyMnrUnts = 1,
                    CcyNames =
                    [
                        new CcyNm { Lang = "EN", Name = "United States Dolar" },
                        new CcyNm { Lang = "LT", Name = "Jungtiniu Amerikos Valstiju Doleris" }
                    ]
                },
                new CcyNtry
                {
                    Ccy = "PLN",
                    CcyNbr = "3",
                    CcyMnrUnts = 1,
                    CcyNames =
                    [
                        new CcyNm { Lang = "EN", Name = "Polish Zlot" },
                        new CcyNm { Lang = "LT", Name = "Lenkijos Zlotas" }
                    ]
                }
            ]
        };
}
