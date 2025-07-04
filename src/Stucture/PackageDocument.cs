using DG.Epub.Constants;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DG.Epub.Stucture
{
    [XmlType("package")]
    [XmlRoot("package")]
    public class PackageDocument
    {
        private BaseTextDirection? _dir;

        [XmlAttribute("dir")]
        public BaseTextDirection Dir
        {
            get
            {
                return _dir ?? BaseTextDirection.Auto;
            }
            set
            {
                _dir = value;
            }
        }

        [XmlIgnore]
        public bool DirSpecified => _dir.HasValue;

        [XmlAttribute("id")]
        public string? Id { get; set; }

        public void WriteTo(XmlWriter xmlWriter)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(PackageDocument), XmlNamespaces.Opf);
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", XmlNamespaces.Opf);

            xsSubmit.Serialize(xmlWriter, this, namespaces);
        }

        public string ToXml()
        {
            var stringbuilder = new StringBuilder();

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (var writer = XmlWriter.Create(stringbuilder, settings))
            {
                WriteTo(writer);
                return stringbuilder.ToString();
            }
        }
    }
}
