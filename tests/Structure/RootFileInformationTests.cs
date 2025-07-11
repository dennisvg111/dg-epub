using DG.Epub.Stucture;
using FluentAssertions;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace DG.Epub.Tests.Structure
{
    public class RootFileInformationTests
    {
        [Fact]
        public void XmlWriter_IncludesAttributes()
        {
            var sb = new StringBuilder();
            XmlSerializer xsSubmit = new XmlSerializer(typeof(RootFileInformation[]));

            using (var xmlWriter = XmlWriter.Create(sb))
            {
                var rootfile = new RootFileInformation("TEST/content.opf", "text/xml");

                xsSubmit.Serialize(xmlWriter, new[] { rootfile });
            }

            var result = sb.ToString();
            result
                .Should().Contain("full-path=\"TEST/content.opf\"")
                .And.Contain("media-type=\"text/xml\"");
        }
    }
}
