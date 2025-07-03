using DG.Epub.Helpers;
using DG.Epub.Logging;
using DG.Epub.Parsing;
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

        private const string _rootfileCollectionName = "rootfiles";
        private static readonly string _rootfileTypeName = XmlHelper.GetXmlTypeName<RootFileInformation>();

        /// <summary>
        /// Gets the default instance of the <see cref="ContainerFile"/> class.
        /// </summary>
        public static ContainerFile Default => new ContainerFile([RootFileInformation.Default]);

        [XmlArray(_rootfileCollectionName)]
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

        public static EpubParsingResult<ContainerFile> Parse(XDocument? xml, EpubLogLevel minimumLogLevel = EpubLogLevel.Informational)
        {
            EpubLogCollectoin logs = new EpubLogCollectoin(minimumLogLevel);
            if (xml == null)
            {
                logs.AddError("container.xml should be a valid XML document.");
                return EPubParsingResult.Completed(new ContainerFile([RootFileInformation.Default]), logs);
            }
            if (xml.Root == null)
            {
                logs.AddError("container.xml document does not have a root element.");
                return EPubParsingResult.Completed(new ContainerFile([RootFileInformation.Default]), logs);
            }

            var xmlNamespace = XNamespace.Get(XmlNamespace);
            var xmlRootFileCollection = xml.Root?.Element(xmlNamespace + _rootfileCollectionName);
            var xmlRootFiles = xmlRootFileCollection?.Elements(xmlNamespace + _rootfileTypeName);

            if (xmlRootFiles == null || !xmlRootFiles.Any())
            {
                logs.AddError("container.xml should contain at least one rootfile (package document).");
                return EPubParsingResult.Completed(new ContainerFile([RootFileInformation.Default]), logs);
            }

            var roots = xmlRootFiles.Select(e => new RootFileInformation(e.Attribute(RootFileInformation.XmlFullPathName).Value, e.Attribute(RootFileInformation.XmlMediaTypeName).Value));

            return EPubParsingResult.Completed(new ContainerFile(roots), logs);
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
