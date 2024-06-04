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
                return null;
            }
        }
    }
}
