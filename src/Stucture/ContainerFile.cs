using DG.Common.Exceptions;
using DG.Epub.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DG.Epub.Stucture
{
    public class ContainerFile
    {
        /// <summary>
        /// The path of the container file.
        /// </summary>
        public const string Path = "META-INF/container.xml";

        private readonly static XNamespace _namespace = "urn:oasis:names:tc:opendocument:xmlns:container";
        private readonly static XName _container = _namespace + "container";
        private readonly static XName _rootFileCollectionElementName = _namespace + "rootfiles";
        private readonly static XName _rootFileElementName = _namespace + "rootfile";

        private readonly List<RootFileInformation> _roots;

        public IReadOnlyList<RootFileInformation> Roots => _roots;

        public ContainerFile(List<RootFileInformation> roots)
        {
            _roots = roots ?? new List<RootFileInformation>();
        }

        public static ContainerFile Parse(XDocument xml)
        {
            ThrowIf.Parameter.IsNull(xml, nameof(xml));
            ThrowIf.Parameter.IsNull(xml.Root, nameof(xml.Root));

            var rootFilesCollection = xml.Root?.Element(_rootFileCollectionElementName);
            var rootFiles = rootFilesCollection?.Elements(_rootFileElementName);
            List<RootFileInformation> roots = rootFiles.Select(e => new RootFileInformation(e.Attribute(RootFileInformation.XmlFullPathName).Value, e.Attribute(RootFileInformation.XmlMediaTypeName).Value)).ToList();

            return new ContainerFile(roots);
        }

        public string ToXml()
        {
            var container = new XElement(_container);
            container.Add(new XAttribute("version", "1.0"));
            container.Add(new XAttribute("xmlns", _namespace));

            var rootfiles = new XElement(_rootFileCollectionElementName);
            foreach (var root in _roots)
            {
                rootfiles.Add(new XElement(_rootFileElementName, new XAttribute(RootFileInformation.XmlFullPathName, root.FullPath), new XAttribute(RootFileInformation.XmlMediaTypeName, root.MediaType)));
            }

            container.Add(rootfiles);
            return container.ToStringWithDeclaration();
        }
    }
}
