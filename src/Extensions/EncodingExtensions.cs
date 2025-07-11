using System.Text;

namespace DG.Epub.Extensions;
public static class EncodingExtensions
{
    public static string ToReadableString(this Encoding encoding)
    {
        if (encoding.GetPreamble().Length > 0)
        {
            return $"{encoding.EncodingName} (BOM)";
        }
        return encoding.EncodingName;
    }
}
