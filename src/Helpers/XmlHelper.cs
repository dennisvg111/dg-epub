using System.Reflection;
using System.Xml.Serialization;

namespace DG.Epub.Helpers
{
    public static class XmlHelper
    {
        public static string GetXmlTypeName<T>()
        {
            var type = typeof(T);
            var xmlTypeAttr = type.GetCustomAttribute<XmlTypeAttribute>();
            return xmlTypeAttr?.TypeName ?? type.Name;
        }
    }
}
