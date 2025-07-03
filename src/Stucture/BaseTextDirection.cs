using System.Xml.Serialization;

namespace DG.Epub.Stucture
{
    /// <summary>
    /// Specifies the base direction [bidi] of the textual content and attribute values of the carrying element and its descendants.
    /// </summary>
    public enum BaseTextDirection
    {
        /// <summary>
        /// left-to-right base direction.
        /// </summary>
        [XmlEnum(Name = "ltr")]
        Ltr,

        /// <summary>
        /// right-to-left base direction.
        /// </summary>
        Rtl,

        /// <summary>
        /// base direction is determined using the Unicode Bidi Algorithm.
        /// </summary>
        Auto
    }
}
