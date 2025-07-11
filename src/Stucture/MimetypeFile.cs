using DG.Epub.Extensions;
using DG.Epub.Logging;
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
        private static readonly int _expectedContentLength = ExpectedMimetype.Length;
        private static readonly Encoding _expectedEncoding = new UTF8Encoding(false);

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
        /// Attempts to parse the content of a stream as a mimetype file.
        /// </summary>
        /// <remarks>
        /// This method reads the stream and validates it. If the content is invalid, errors are logged via the given <paramref name="logWriter"/>.
        /// </remarks>
        /// <param name="stream">The input stream containing the mimetype file data. The stream must be readable and positioned at the start of the file.</param>
        /// <param name="logWriter">An instance of <see cref="IEpubLogWriter"/> used to log errors and warnings encountered during parsing.</param>
        /// <param name="mimetypeFile">When this method returns, contains the parsed <see cref="MimetypeFile"/> object if parsing succeeds, or an invalid <see cref="MimetypeFile"/> object if parsing fails.</param>
        /// <returns><see langword="true"/> if the parsing operation completes successfully; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(Stream stream, IEpubLogWriter logWriter, out MimetypeFile mimetypeFile)
        {
            using (StreamReader reader = new StreamReader(stream, _expectedEncoding))
            {
                char[] chars = new char[_expectedContentLength];
                int read = reader.ReadBlock(chars, 0, _expectedContentLength);

                if (read != _expectedContentLength)
                {
                    logWriter.AddError($"Expected {_expectedContentLength} characters in the mimetype file, but only found {read}.");
                }
                if (!reader.EndOfStream)
                {
                    logWriter.AddError($"Expected {_expectedContentLength} characters in the mimetype file, but found more.");
                }
                if (!Equals(reader.CurrentEncoding, _expectedEncoding))
                {
                    logWriter.AddWarning($"Expected mimetype file to have encoding '{_expectedEncoding.ToReadableString()}', but detected encoding was '{reader.CurrentEncoding.ToReadableString()}'.");
                }
                string content = new string(chars).Substring(0, read);
                mimetypeFile = new MimetypeFile(content);

                if (!mimetypeFile.IsValid)
                {
                    logWriter.AddError($"The mimetype file contains invalid content: '{content}'. Expected content is '{ExpectedMimetype}'");
                }

                return true;
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
