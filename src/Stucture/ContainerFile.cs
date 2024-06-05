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

        private readonly List<RootFile> _roots;

        public IReadOnlyList<RootFile> Roots => _roots;

        public ContainerFile(List<RootFile> roots)
        {
            _roots = roots ?? new List<RootFile>();
        }

        public static ContainerFile Parse(XDocument xml)
        {
            ThrowIf.Parameter.IsNull(xml, nameof(xml));
            ThrowIf.Parameter.IsNull(xml.Root, nameof(xml.Root));

            var rootFilesCollection = xml.Root?.Element(_rootFileCollectionElementName);
            var rootFiles = rootFilesCollection?.Elements(_rootFileElementName);
            List<RootFile> roots = rootFiles.Select(e => new RootFile(e.Attribute(RootFile.XmlFullPathName).Value, e.Attribute(RootFile.XmlMediaTypeName).Value)).ToList();

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
                rootfiles.Add(new XElement(_rootFileElementName, new XAttribute(RootFile.XmlFullPathName, root.FullPath), new XAttribute(RootFile.XmlMediaTypeName, root.MediaType)));
            }

            container.Add(rootfiles);
            return container.ToStringWithDeclaration();
        }
    }
}
