using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DG.Epub.Stucture
{
    [XmlType("container")]
    [XmlRoot("container")]
    public class ContainerFile
    {
        /// <summary>
        /// The XML namespace for the container format.
        /// </summary>
        public const string XmlNamespace = "urn:oasis:names:tc:opendocument:xmlns:container";

        public const string XmlRootfileCollectionName = "rootfiles";

        /// <summary>
        /// Gets the default instance of the <see cref="ContainerFile"/> class.
        /// </summary>
        public static ContainerFile Default => new ContainerFile([RootFileInformation.Default]);

        [XmlArray(XmlRootfileCollectionName)]
        public RootFileInformation[] Roots { get; set; }

        /// <summary>
        /// The version of the container file format.
        /// </summary>
        [XmlAttribute("version")]
        public string Version { get; set; } = "1.0";

        public ContainerFile(IEnumerable<RootFileInformation> roots)
        {
            if (roots != null)
            {
                Roots = roots.ToArray();
            }
            else
            {
                Roots = Array.Empty<RootFileInformation>();
            }
        }

        // Required for XML serialization, this constructor is not intended for use in code.
        private ContainerFile()
        {
            Roots = Array.Empty<RootFileInformation>();
        }

        public void WriteTo(XmlWriter xmlWriter)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(ContainerFile), XmlNamespace);
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", XmlNamespace);

            xsSubmit.Serialize(xmlWriter, this, namespaces);
        }

        public string ToXml()
        {
            var stringbuilder = new StringBuilder();
            stringbuilder.AppendLine(new XDeclaration("1.0", "UTF-8", null).ToString());

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
