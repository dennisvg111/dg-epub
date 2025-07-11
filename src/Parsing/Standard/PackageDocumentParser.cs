using DG.Epub.Logging;
using DG.Epub.Stucture;
using System.IO.Compression;

namespace DG.Epub.Parsing.Standard;

public class PackageDocumentParser : IEpubComponentParser<PackageDocument>
{
    public void AddToBook(EpubBook book, PackageDocument? data)
    {
        book.PackageDocument = data;
    }

    public bool TryParse(ZipArchive zip, IEpubLogWriter logWriter, out PackageDocument? data)
    {
        throw new System.NotImplementedException();
    }
}
