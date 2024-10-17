using System.Xml.Serialization;

namespace CurrencyConverter.Domain.Helpers;

public static class XmlSerializerHelper
{
    public static T? DeserializeXml<T>(this string document)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringReader = new StringReader(document);
        return (T?)serializer.Deserialize(stringReader);
    }
}
