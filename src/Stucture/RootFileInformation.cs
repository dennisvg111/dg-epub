using System;

namespace DG.Epub.Stucture
{
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

        private readonly string _fullPath;
        private readonly string _mediaType;

        /// <summary>
        /// The path of this <see cref="RootFileInformation"/>.
        /// </summary>
        public string FullPath => _fullPath;

        /// <summary>
        /// The media type of this <see cref="RootFileInformation"/>.
        /// </summary>
        public string MediaType => _mediaType;

        public RootFileInformation(string fullPath, string mediaType)
        {
            _fullPath = fullPath;
            _mediaType = mediaType;
        }

        #region static instances
        private static readonly Lazy<RootFileInformation> _default = new Lazy<RootFileInformation>(() => new RootFileInformation(DefaultPath, DefaultMediaType));

        /// <summary>
        /// Returns the default instance of <see cref="RootFileInformation"/>, with <see cref="FullPath"/> set to <see cref="DefaultPath"/> and <see cref="MediaType"/> set to <see cref="DefaultMediaType"/>
        /// </summary>
        public static RootFileInformation Default => _default.Value;
        #endregion
    }
}
