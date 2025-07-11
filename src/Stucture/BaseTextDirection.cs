using System.Xml.Serialization;

namespace DG.Epub.Stucture
{
    /// <summary>
    /// Specifies the base direction (bidi) of the textual content and attribute values of the carrying element and its descendants.
    /// </summary>
    public enum BaseTextDirection
    {

        /// <summary>
        /// Base direction is determined using the Unicode Bidi Algorithm.
        /// </summary>
        [XmlEnum(Name = "auto")]
        Auto,

        /// <summary>
        /// Left-to-right base direction.
        /// </summary>
        [XmlEnum(Name = "ltr")]
        Ltr,

        /// <summary>
        /// Right-to-left base direction.
        /// </summary>
        [XmlEnum(Name = "rtl")]
        Rtl,
    }
}
