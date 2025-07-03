using DG.Epub.Extensions;
using DG.Epub.Logging;
using DG.Epub.Stucture;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace DG.Epub.Parsing.Standard;

internal class ContainerParser : IEpubComponentParser<ContainerFile>
{
    public void AddToBook(EpubBook book, ContainerFile? data)
    {
        book.ContainerFile = data;
    }

    public bool TryParse(ZipArchive zip, IEpubLogWriter logWriter, out ContainerFile? data)
    {
        if (!zip.TryFindEntry(EpubPaths.Container, out var entry))
        {
            logWriter.AddError($"The EPUB file does not contain a required entry at path '{EpubPaths.Container}'.");
            data = null;
            return false;
        }

        var xml = entry.GetXml();

        return TryParse(xml, logWriter, out data);
    }

    public bool TryParse(XDocument xml, IEpubLogWriter logWriter, out ContainerFile? data)
    {
        if (xml == null)
        {
            logWriter.AddFatal("container.xml should be a valid XML document.");
            data = ContainerFile.Default;
            return false;
        }
        if (xml.Root == null)
        {
            logWriter.AddFatal("container.xml document does not have a root element.");
            data = ContainerFile.Default;
            return false;
        }
        CheckContainerElement(xml.Root, logWriter);

        if (!TryGetRootFiles(xml.Root, out var xmlRootFiles))
        {
            logWriter.AddFatal("container.xml should contain at least one rootfile (package document).");
            data = ContainerFile.Default;
            return false;
        }

        var roots = xmlRootFiles.Select(e => ParseRootFile(e, logWriter));
        if (roots.All(r => string.IsNullOrEmpty(r.FullPath)))
        {
            logWriter.AddFatal($"container.xml rootfile should contain full-path attribute with value.");
            data = ContainerFile.Default;
            return false;
        }

        data = new ContainerFile(roots);
        return true;
    }

    private static bool TryGetRootFiles(XElement xmlContainer, out IEnumerable<XElement>? xmlRootFiles)
    {
        var xmlNamespace = XNamespace.Get(ContainerFile.XmlNamespace);
        var xmlRootFileCollection = xmlContainer?.Element(xmlNamespace + ContainerFile.XmlRootfileCollectionName);
        xmlRootFiles = xmlRootFileCollection?.Elements(xmlNamespace + RootFileInformation.XmlTypeName);

        if (xmlRootFiles == null || !xmlRootFiles.Any())
        {
            return false;
        }

        return true;
    }

    private static RootFileInformation ParseRootFile(XElement element, IEpubLogWriter logWriter)
    {
        var fullPathAttribute = element.Attribute(RootFileInformation.XmlFullPathName);
        var mediatypeAttribute = element.Attribute(RootFileInformation.XmlMediaTypeName);

        if (mediatypeAttribute == null || string.IsNullOrEmpty(mediatypeAttribute.Value))
        {
            logWriter.AddWarning("container.xml rootfile should contain media-type attribute.");
        }
        var mediatype = mediatypeAttribute?.Value;
        if (!RootFileInformation.DefaultMediaType.Equals(mediatype))
        {
            logWriter.AddWarning($"Expected media-type attribute value to be '{RootFileInformation.DefaultMediaType}' but found '{mediatype}' instead.");
        }

        if (fullPathAttribute == null || string.IsNullOrEmpty(fullPathAttribute.Value))
        {
            logWriter.AddError("container.xml rootfile should contain full-path attribute.");
        }
        var fullPath = fullPathAttribute?.Value;

        return new RootFileInformation(fullPath ?? string.Empty, mediatype ?? RootFileInformation.DefaultMediaType);
    }

    private static void CheckContainerElement(XElement container, IEpubLogWriter logWriter)
    {
        var versionAttribute = container.Attribute("version");
        if (versionAttribute == null)
        {
            logWriter.AddWarning("Container element must have a version attribute.");
            return;
        }

        if (string.IsNullOrEmpty(versionAttribute.Value))
        {
            logWriter.AddWarning("Container element version attribute must have value.");
            return;
        }

        if (versionAttribute.Value != "1.0")
        {
            logWriter.AddWarning("Container element version attribute must be '1.0'.");
            return;
        }
    }
}
