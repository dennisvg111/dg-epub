using System;
using System.Text;
using System.Xml.Linq;

namespace DG.Epub.Extensions
{
    public static class XElementExtensions
    {
        private static readonly XDeclaration _defaultDeclaration = new XDeclaration("1.0", Encoding.UTF8.WebName, null);

        public static string ToStringWithDeclaration(this XElement xml, SaveOptions options = SaveOptions.None)
        {
            var newLine = (options & SaveOptions.DisableFormatting) == SaveOptions.DisableFormatting ? "" : Environment.NewLine;
            return _defaultDeclaration + newLine + xml.ToString(options);
        }
    }
}
