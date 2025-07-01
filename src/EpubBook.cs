using DG.Epub.ErrorDetection;
using DG.Epub.Extensions;
using DG.Epub.Stucture;
using System.IO;
using System.IO.Compression;

namespace DG.Epub
{
    public class EpubBook
    {

        public static EPubParsingResult<EpubBook> FromStream(Stream s)
        {
            using (ZipArchive zip = new ZipArchive(s, ZipArchiveMode.Read, false, null))
            {
                if (!zip.TryFindEntry(ContainerFile.Path, out var entry))
                {
                    return null;
                }
                var containerXml = entry.GetXml();
                var containerResult = ContainerFile.Parse(containerXml);
                if (containerResult.HasFatalError)
                {
                    return new EPubParsingResult<EpubBook>(null, containerResult);
                }
            }
        }
    }
}
