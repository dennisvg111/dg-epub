using DG.Epub.ErrorDetection;
using DG.Epub.Extensions;
using DG.Epub.Stucture;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace DG.Epub;

/// <summary>
/// Represents an ePub book, providing functionality to parse and interact with ePub file data.
/// </summary>
/// <remarks>
/// <para>This class serves as the main entry point for working with ePub files. It provides methods to parse ePub data from streams and access the contents of the book.</para>
/// <para>Use the <see cref="FromStream(Stream, EPubLogLevel)"/> method to create an instance of <see cref="EpubBook"/> from a valid ePub file.</para>
/// </remarks>
public class EpubBook
{
    /// <summary>
    /// Gets or sets the MIME type file associated with the current ePub book.
    /// </summary>
    /// <remarks>This should usually be <see cref="MimetypeFile.Default"/>.</remarks>
    public MimetypeFile? MimetypeFile { get; set; }

    /// <summary>
    /// Gets or sets the container file associated with the current ePub book.
    /// </summary>
    public ContainerFile? ContainerFile { get; set; }

    /// <summary>
    /// Parses an ePub file from the provided stream and returns the result of the parsing operation.
    /// </summary>
    /// <param name="s">The <see cref="Stream"/> containing the ePub file data. The stream must be readable and positioned at the beginning of the ePub file.</param>
    /// <param name="minimumLogLevel"></param>
    /// <returns>An <see cref="EPubParsingResult{T}"/> containing the parsed <see cref="EpubBook"/> if the operation succeeds, or an error result if the ePub file is invalid or cannot be parsed.</returns>
    public static EPubParsingResult<EpubBook> FromStream(Stream s, EPubLogLevel minimumLogLevel = EPubLogLevel.Informational)
    {
        EPubLogCollection logs = new EPubLogCollection(minimumLogLevel);

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

            return EPubParsingResult.Success(book, logs);
        }
    }

    private static bool TryParsePart<T>(EPubParsingResult<T> result, EPubLogCollection logs, out T? value)
    {
        return result.AndCopyLogsTo(logs).TryGetValue(out value);
    }

    private static EPubParsingResult<T> ParseXml<T>(ZipArchive zip, string path, Func<XDocument?, EPubParsingResult<T>> parseXmlFunc)
    {
        return ParseEntry(zip, path, (entry) =>
        {
            var xml = entry.GetXml();
            return parseXmlFunc(xml);
        });
    }

    private static EPubParsingResult<T> ParseStream<T>(ZipArchive zip, string path, Func<Stream, EPubParsingResult<T>> parseFunc)
    {
        return ParseEntry(zip, path, (entry) =>
        {
            using (var stream = entry.Open())
            {
                return parseFunc(stream);
            }
        });
    }

    private static EPubParsingResult<T> ParseEntry<T>(ZipArchive zip, string path, Func<ZipArchiveEntry, EPubParsingResult<T>> parseFunc)
    {
        if (!zip.TryFindEntry(path, out var entry))
        {
            return EPubParsingResult.FatalFor<T>($"The ePub file does not contain a required entry at path '{path}'.");
        }

        return parseFunc(entry);
    }
}