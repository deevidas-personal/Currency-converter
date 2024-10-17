using System.Xml.Serialization;

namespace CurrencyConverter.Domain.Models;

[XmlRoot(ElementName = "FxRates", Namespace = "http://www.lb.lt/WebServices/FxRates")]
public class FxRates
{
    [XmlElement(ElementName = "FxRate")]
    public List<FxRate> FxRateList { get; set; }
}

public class FxRate
{
    [XmlElement(ElementName = "Tp")]
    public string Type { get; set; }

    [XmlElement(ElementName = "Dt")]
    public DateTime Date { get; set; }

    [XmlElement(ElementName = "CcyAmt")]
    public List<CcyAmt> CurrencyAmounts { get; set; }
}

public class CcyAmt
{
    [XmlElement(ElementName = "Ccy")]
    public string Currency { get; set; }

    [XmlElement(ElementName = "Amt")]
    public decimal Amount { get; set; }
}
