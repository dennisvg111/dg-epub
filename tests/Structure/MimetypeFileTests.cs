using DG.Epub.Stucture;
using FluentAssertions;
using System.IO;
using System.Text;
using Xunit;

namespace DG.Epub.Tests.Structure
{
    public class MimetypeFileTests
    {
        private MemoryStream TextAsStream(string content, Encoding encoding = null)
        {
            var stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, encoding ?? Encoding.ASCII);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [Fact]
        public void TryParse_Correct_ReturnsTrue()
        {
            using (var stream = TextAsStream(MimetypeFile.ExpectedMimetype))
            {
                var bytes = stream.ToArray();
                bool canParse = MimetypeFile.TryParse(stream, out MimetypeFile mimetype);

                canParse.Should().BeTrue();
            }
        }

        [Fact]
        public void TryParse_BomPrefix_ReturnsFalse()
        {
            using (var stream = TextAsStream(MimetypeFile.ExpectedMimetype, new UTF8Encoding(true)))
            {
                var bytes = stream.ToArray();
                bool canParse = MimetypeFile.TryParse(stream, out MimetypeFile mimetype);

                canParse.Should().BeFalse();
            }
        }
    }
}
