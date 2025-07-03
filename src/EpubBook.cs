using DG.Epub.Extensions;
using DG.Epub.Logging;
using DG.Epub.Parsing;
using DG.Epub.Stucture;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace DG.Epub;

/// <summary>
/// Represents an EPUB book, providing functionality to parse and interact with EPUB file data.
/// </summary>
/// <remarks>
/// <para>This class serves as the main entry point for working with EPUB files. It provides methods to parse EPUB data from streams and access the contents of the book.</para>
/// <para>Use the <see cref="FromStream(Stream, EpubLogLevel)"/> method to create an instance of <see cref="EpubBook"/> from a valid EPUB file.</para>
/// </remarks>
public class EpubBook
{
    /// <summary>
    /// Gets or sets the MIME type file associated with the current EPUB book.
    /// </summary>
    /// <remarks>This should usually be <see cref="MimetypeFile.Default"/>.</remarks>
    public MimetypeFile? MimetypeFile { get; set; }

    /// <summary>
    /// Gets or sets the container file associated with the current EPUB book.
    /// </summary>
    public ContainerFile? ContainerFile { get; set; }

    /// <summary>
    /// Parses an EPUB file from the provided stream and returns the result of the parsing operation.
    /// </summary>
    /// <param name="s">The <see cref="Stream"/> containing the EPUB file data. The stream must be readable and positioned at the beginning of the EPUB file.</param>
    /// <param name="minimumLogLevel"></param>
    /// <returns>An <see cref="EpubParsingResult{T}"/> containing the parsed <see cref="EpubBook"/> if the operation succeeds, or an error result if the EPUB file is invalid or cannot be parsed.</returns>
    public static EpubParsingResult<EpubBook> FromStream(Stream s, EpubLogLevel minimumLogLevel = EpubLogLevel.Informational)
    {
        var logs = new EpubLogCollectoin(minimumLogLevel);

        var book = new EpubBook();

        using (ZipArchive zip = new ZipArchive(s, ZipArchiveMode.Read, false, null))
        {
            if (!TryParsePart(ParseStream(zip, EpubPaths.Mimetype, s => MimetypeFile.Parse(s, minimumLogLevel)), logs, out var mimetype))
            {
                return EPubParsingResult.FatalFor<EpubBook>(logs);
            }
            book.MimetypeFile = mimetype;

            if (!TryParsePart(ParseXml(zip, EpubPaths.Container, x => ContainerFile.Parse(x, minimumLogLevel)), logs, out var container))
            {
                return EPubParsingResult.FatalFor<EpubBook>(logs);
            }
            book.ContainerFile = container;

            return EPubParsingResult.Completed(book, logs);
        }
    }

    private static bool TryParsePart<T>(EpubParsingResult<T> result, EpubLogCollectoin logs, out T? value)
    {
        return result.AndCopyLogsTo(logs).TryGetValue(out value);
    }

    private static EpubParsingResult<T> ParseXml<T>(ZipArchive zip, string path, Func<XDocument?, EpubParsingResult<T>> parseXmlFunc)
    {
        return ParseEntry(zip, path, (entry) =>
        {
            var xml = entry.GetXml();
            return parseXmlFunc(xml);
        });
    }

    private static EpubParsingResult<T> ParseStream<T>(ZipArchive zip, string path, Func<Stream, EpubParsingResult<T>> parseFunc)
    {
        return ParseEntry(zip, path, (entry) =>
        {
            using (var stream = entry.Open())
            {
                return parseFunc(stream);
            }
        });
    }

    private static EpubParsingResult<T> ParseEntry<T>(ZipArchive zip, string path, Func<ZipArchiveEntry, EpubParsingResult<T>> parseFunc)
    {
        if (!zip.TryFindEntry(path, out var entry))
        {
            return EPubParsingResult.FatalFor<T>($"The EPUB file does not contain a required entry at path '{path}'.");
        }

        return parseFunc(entry);
    }
}