using DG.Epub.Extensions;
using DG.Epub.Stucture;
using System.IO;
using System.IO.Compression;

namespace DG.Epub
{
    public class EpubBook
    {

        public static EpubBook FromStream(Stream s)
        {
            using (ZipArchive zip = new ZipArchive(s, ZipArchiveMode.Read, false, null))
            {
                ContainerFile file;
                if (!zip.TryGetEntry(ContainerFile.Path, out var entry))
                {
                    return null;
                }
                var containerXml = entry.GetXml();
                file = ContainerFile.Parse(containerXml);
                return null;
            }
        }
    }
}
