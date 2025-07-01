using System;
using System.Xml.Serialization;

namespace DG.Epub.Stucture
{
    /// <summary>
    /// Represents information about a root file, including its path and media type.
    /// </summary>
    /// <remarks>
    /// A default instance of this class can be obtained via the <see cref="Default"/> property.
    /// </remarks>
    [XmlType("rootfile")]
    public class RootFileInformation
    {
        /// <summary>
        /// The name of the xml attribute that indicates the path of this root file.
        /// </summary>
        public const string XmlFullPathName = "full-path";

        /// <summary>
        /// The name of the xml attribute that indicates the media type of this root file.
        /// </summary>
        public const string XmlMediaTypeName = "media-type";

        /// <summary>
        /// OEBPS/content.opf
        /// </summary>
        public const string DefaultPath = "OEBPS/content.opf";

        /// <summary>
        /// application/oebps-package+xml
        /// </summary>
        public const string DefaultMediaType = "application/oebps-package+xml";

        /// <summary>
        /// The path of this <see cref="RootFileInformation"/>.
        /// </summary>
        [XmlAttribute(XmlFullPathName)]
        public string FullPath { get; set; }

        /// <summary>
        /// The media type of this <see cref="RootFileInformation"/>.
        /// </summary>
        [XmlAttribute(XmlMediaTypeName)]
        public string MediaType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RootFileInformation"/> class with the specified <paramref name="fullPath"/> and <paramref name="mediaType"/>.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="mediaType"></param>
        public RootFileInformation(string fullPath, string mediaType)
        {
            FullPath = fullPath;
            MediaType = mediaType;
        }

        // Required for XML serialization, this constructor is not intended for use in code.
        private RootFileInformation() { }

        #region static instances
        private static readonly Lazy<RootFileInformation> _default = new Lazy<RootFileInformation>(() => new RootFileInformation(DefaultPath, DefaultMediaType));

        /// <summary>
        /// Returns the default instance of <see cref="RootFileInformation"/>, with <see cref="FullPath"/> set to <see cref="DefaultPath"/> and <see cref="MediaType"/> set to <see cref="DefaultMediaType"/>
        /// </summary>
        public static RootFileInformation Default => _default.Value;
        #endregion
    }
}
