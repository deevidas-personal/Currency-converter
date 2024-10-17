using System.Xml.Serialization;

namespace CurrencyConverter.Domain.Models;

[XmlRoot("CcyTbl", Namespace ="http://www.lb.lt/WebServices/FxRates")]
public class CcyTbl
{
    [XmlElement("CcyNtry")]
    public List<CcyNtry> CcyNtries { get; set; }
}

public class CcyNtry
{
    [XmlElement("Ccy")]
    public string Ccy { get; set; }

    [XmlElement("CcyNbr")]
    public string CcyNbr { get; set; }

    [XmlElement("CcyMnrUnts")]
    public int CcyMnrUnts { get; set; }

    // List for handling multiple CcyNm elements with attributes
    [XmlElement("CcyNm")]
    public List<CcyNm> CcyNames { get; set; }
}

public class CcyNm
{
    [XmlAttribute("lang")]
    public string Lang { get; set; }

    [XmlText]
    public string Name { get; set; }
}
