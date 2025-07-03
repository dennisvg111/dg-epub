using DG.Epub.Extensions;
using DG.Epub.Logging;
using DG.Epub.Stucture;
using System.IO.Compression;

namespace DG.Epub.Parsing.Standard;

/// <summary>
/// Provides functionality for parsing and handling the mimetype file within an EPUB archive.
/// </summary>
public class MimetypeParser : IEpubComponentParser<MimetypeFile>
{
    /// <inheritdoc/>
    public void AddToBook(EpubBook book, MimetypeFile? data)
    {
        book.MimetypeFile = data;
    }

    /// <inheritdoc/>
    public bool TryParse(ZipArchive zip, IEpubLogWriter logWriter, out MimetypeFile? data)
    {
        if (!zip.TryFindEntry(EpubPaths.Mimetype, out var entry))
        {
            logWriter.AddError($"The EPUB file does not contain a required entry at path '{EpubPaths.Mimetype}'.");

            data = MimetypeFile.Default;
            return true;
        }

        using (var stream = entry.Open())
        {
            return MimetypeFile.TryParse(stream, logWriter, out data);
        }
    }
}
