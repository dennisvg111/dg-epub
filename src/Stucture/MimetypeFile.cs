using System.IO;
using System.Text;

namespace DG.Epub.Stucture
{
    public class MimetypeFile
    {
        /// <summary>
        /// The path of the container file.
        /// </summary>
        public const string Path = "mimetype";

        /// <summary>
        /// The expected content of the mimetype file.
        /// </summary>
        public const string ExpectedMimetype = "application/epub+zip";
        private const int _expectedContentLength = 20;
        private readonly static Encoding _encoding = new UTF8Encoding(false);

        private readonly string _mimetype;

        public bool IsValid => _mimetype == ExpectedMimetype;

        public MimetypeFile(string mimetype)
        {
            _mimetype = mimetype;
        }

        public static bool TryParse(Stream stream, out MimetypeFile mimetype)
        {
            using (StreamReader reader = new StreamReader(stream, _encoding))
            {
                char[] chars = new char[_expectedContentLength];
                int read = reader.ReadBlock(chars, 0, _expectedContentLength);
                if (read != _expectedContentLength || !reader.EndOfStream || !Equals(reader.CurrentEncoding, _encoding))
                {
                    mimetype = null;
                    return false;
                }
                string content = new string(chars);
                mimetype = new MimetypeFile(content);

                return mimetype.IsValid;
            }
        }
    }
}
