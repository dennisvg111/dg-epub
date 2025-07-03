using DG.Epub.Extensions;
using DG.Epub.Logging;
using DG.Epub.Parsing;
using System;
using System.IO;
using System.Text;

namespace DG.Epub.Stucture
{
    /// <summary>
    /// Represents the MIME type file in an EPUB container, providing functionality to parse, validate, and access its content.
    /// </summary>
    public class MimetypeFile
    {
        /// <summary>
        /// The expected content of the mimetype file.
        /// </summary>
        public const string ExpectedMimetype = "application/epub+zip";
        private const int _expectedContentLength = 20;
        private readonly static Encoding _encoding = new UTF8Encoding(false);

        private readonly string _mimetype;

        /// <summary>
        /// Gets the MIME type defined in this EPUB.
        /// </summary>
        public string Mimetype => _mimetype;

        /// <summary>
        /// Gets a value indicating whether the current object is valid based on its MIME type.
        /// </summary>
        public bool IsValid => _mimetype == ExpectedMimetype;

        /// <summary>
        /// Initializes a new instance of the <see cref="MimetypeFile"/> class with the specified MIME type.
        /// </summary>
        /// <param name="mimetype">The MIME type associated with the file. Cannot be null or empty.</param>
        public MimetypeFile(string mimetype)
        {
            _mimetype = mimetype;
        }

        /// <summary>
        /// Parses the provided <paramref name="stream"/> to extract and validate the mimetype file content.
        /// </summary>
        /// <remarks>The method reads the mimetype file from the provided stream, validates its content and encoding, and logs any warnings or errors encountered during parsing.</remarks>
        /// <param name="stream">The input stream containing the mimetype file data. The stream must be readable and positioned at the start of the mimetype file.</param>
        /// <param name="miminumLogLevel">The minimum log level for capturing parsing logs. Defaults to <see cref="EpubLogLevel.Informational"/>.</param>
        /// <returns>An <see cref="EpubParsingResult{MimetypeFile}"/> containing the parsed <see cref="MimetypeFile"/> object and any associated logs.</returns>
        public static EpubParsingResult<MimetypeFile> Parse(Stream stream, EpubLogLevel miminumLogLevel = EpubLogLevel.Informational)
        {
            EpubLogCollectoin logs = new EpubLogCollectoin(miminumLogLevel);
            using (StreamReader reader = new StreamReader(stream, _encoding))
            {
                char[] chars = new char[_expectedContentLength];
                int read = reader.ReadBlock(chars, 0, _expectedContentLength);

                if (read != _expectedContentLength)
                {
                    logs.AddError($"Expected {_expectedContentLength} characters in the mimetype file, but only found {read}.");
                }
                if (!reader.EndOfStream)
                {
                    logs.AddError($"Expected {_expectedContentLength} characters in the mimetype file, but found more.");
                }
                if (!Equals(reader.CurrentEncoding, _encoding))
                {
                    logs.AddWarning($"Expected mimetype file to have encoding '{_encoding.ToReadableString()}', but detected encoding was '{reader.CurrentEncoding.ToReadableString()}'.");
                }
                string content = new string(chars).Substring(0, read);
                var mimetype = new MimetypeFile(content);

                if (!mimetype.IsValid)
                {
                    logs.AddError($"The mimetype file contains invalid content: '{content}'.");
                }
                return new EpubParsingResult<MimetypeFile>(mimetype, logs);
            }
        }

        #region static instances
        private static readonly Lazy<MimetypeFile> _default = new Lazy<MimetypeFile>(() => new MimetypeFile(ExpectedMimetype));

        /// <summary>
        /// Returns the default instance of <see cref="MimetypeFile"/>, with <see cref="Mimetype"/> set to <see cref="ExpectedMimetype"/>.
        /// </summary>
        public static MimetypeFile Default => _default.Value;
        #endregion
    }
}
