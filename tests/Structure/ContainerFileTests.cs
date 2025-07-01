using DG.Epub.Stucture;
using DG.Epub.Tests.Extensions;
using FluentAssertions;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace DG.Epub.Tests.Structure
{
    public class ContainerFileTests
    {
        private const string containerWithSingleFile = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">\r\n  <rootfiles>\r\n    <rootfile full-path=\"OEBPS/content.opf\" media-type=\"application/oebps-package+xml\"/>\r\n  </rootfiles>\r\n</container>";
        private const string containerWithTwoFiles = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">\r\n  <rootfiles>\r\n    <rootfile full-path=\"OEBPS/content.opf\" media-type=\"application/oebps-package+xml\"/>\r\n    <rootfile full-path=\"OEBPS/content2.opf\" media-type=\"application/oebps-package+xml\"/>\r\n  </rootfiles>\r\n</container>";

        [Fact]
        public void Parse_SetsFullPath()
        {
            using (StringReader reader = new StringReader(containerWithSingleFile))
            using (XmlReader xmlReader = XmlReader.Create(reader, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                var document = XDocument.Load(xmlReader);

                var result = ContainerFile.Parse(document).Value;

                result.Roots.Should().ContainSingle();
                result.Roots[0].FullPath.Should().Be("OEBPS/content.opf");
            }
        }

        [Fact]
        public void Parse_SetsMediaType()
        {
            using (StringReader reader = new StringReader(containerWithSingleFile))
            using (XmlReader xmlReader = XmlReader.Create(reader, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                var document = XDocument.Load(xmlReader);

                var result = ContainerFile.Parse(document).Value;

                result.Roots.Should().ContainSingle();
                result.Roots[0].MediaType.Should().Be("application/oebps-package+xml");
            }
        }

        [Fact]
        public void Parse_AllowsMultipleFiles()
        {
            using (StringReader reader = new StringReader(containerWithTwoFiles))
            using (XmlReader xmlReader = XmlReader.Create(reader, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                var document = XDocument.Load(xmlReader);

                var result = ContainerFile.Parse(document).Value;

                result.Roots.Should().HaveCount(2);
            }
        }

        [Fact]
        public void ToXml_ReturnsCorrectString()
        {
            using (StringReader reader = new StringReader(containerWithSingleFile))
            using (XmlReader xmlReader = XmlReader.Create(reader, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {
                var document = XDocument.Load(xmlReader);

                var result = ContainerFile.Parse(document).Value;
                var xml = result.ToXml();

                xml.Should().BeSameIgnoringWhitespace(containerWithSingleFile);
            }
        }

        [Fact]
        public void WriteTo_CanBeParsed()
        {
            var containerFile = new ContainerFile(new[]
            {
                new RootFileInformation("Test/content.opf", "application.test")
            });

            var sb = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(sb, new XmlWriterSettings() { Encoding = Encoding.UTF8 }))
            {
                containerFile.WriteTo(xmlWriter);

                var xml = sb.ToString();

                var containerFile2 = ContainerFile.Parse(XDocument.Parse(xml)).Value;

                containerFile2.Should().BeEquivalentTo(containerFile);
            }
        }
    }
}
