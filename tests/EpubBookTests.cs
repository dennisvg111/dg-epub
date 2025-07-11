using DG.Epub.Parsing;
using FluentAssertions;
using System.IO;
using Xunit;

namespace DG.Epub.Tests
{
    public class EpubBookTests
    {
        private static readonly EpubParsingPipeline pipeline = EpubParsingPipeline.Default;

        [Fact]
        public void FromStream_Tests()
        {
            using (var exampleStream = File.OpenRead("minimal-v3.epub"))
            {
                var bookResult = pipeline.Parse(exampleStream);

                bookResult.HasFatalError.Should().BeFalse();

                bookResult.TryGetValue(out EpubBook book).Should().BeTrue();

                book.ContainerFile.Roots[0].FullPath.Should().Be("EPUB/package.opf");
            }
        }
    }
}
