using DG.Epub.Stucture;
using FluentAssertions;
using Xunit;

namespace DG.Epub.Tests.Structure
{
    public class PackageDocumentTests
    {
        [Fact]
        public void ToXml_DirSet_IncludesDirAttribute()
        {
            var document = new PackageDocument()
            {
                Dir = BaseTextDirection.Rtl,
            };

            var xml = document.ToXml();

            xml.Should().Contain(" dir=\"rtl\"");
        }

        [Fact]
        public void ToXml_DirNotSet_DoesNotIncludeAttribute()
        {
            var document = new PackageDocument();

            var xml = document.ToXml();

            xml.Should().NotContain(" dir");
        }
    }
}
