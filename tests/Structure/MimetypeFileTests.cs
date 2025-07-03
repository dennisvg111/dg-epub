using DG.Epub.Logging;
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
        public void Parse_Expected_GivesAtMostInformation()
        {
            using (var stream = TextAsStream(MimetypeFile.ExpectedMimetype))
            {
                EpubLogCollection log = new EpubLogCollection();

                MimetypeFile.TryParse(stream, log, out MimetypeFile _).Should().BeTrue();

                log.HighestSeverity.Should().Match(l => l <= EpubLogLevel.Informational);
            }
        }

        [Fact]
        public void Parse_BomPrefix_GivesWarning()
        {
            using (var stream = TextAsStream(MimetypeFile.ExpectedMimetype, new UTF8Encoding(true)))
            {
                EpubLogCollection log = new EpubLogCollection();

                MimetypeFile.TryParse(stream, log, out MimetypeFile _).Should().BeTrue();

                log.HighestSeverity.Should().Match(l => l >= EpubLogLevel.Warning);
            }
        }

        [Fact]
        public void Parse_ShorterMimetype_GivesError()
        {
            using (var stream = TextAsStream("application/zip"))
            {
                EpubLogCollection log = new EpubLogCollection();

                MimetypeFile.TryParse(stream, log, out MimetypeFile _).Should().BeTrue();

                log.HighestSeverity.Should().Match(l => l >= EpubLogLevel.Error);
            }
        }

        [Fact]
        public void Parse_LongerMimetype_GivesError()
        {
            using (var stream = TextAsStream(MimetypeFile.ExpectedMimetype + "+txt"))
            {
                EpubLogCollection log = new EpubLogCollection();

                MimetypeFile.TryParse(stream, log, out MimetypeFile _).Should().BeTrue();

                log.HighestSeverity.Should().Match(l => l >= EpubLogLevel.Error);
            }
        }
    }
}
