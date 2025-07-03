using DG.Epub.Logging;
using System.IO.Compression;

namespace DG.Epub.Parsing;

/// <summary>
/// Defines methods for interacting with an EPUB parser, providing methods to retrieve parser information and process EPUB data.
/// </summary>
internal interface IEpubParsingPipelineStep
{
    /// <summary>
    /// Retrieves the name of the underlying parser.
    /// </summary>
    /// <returns>A string representing the name of the parser. The returned value should never be <see langword="null"/> or empty.</returns>
    string ParserName { get; }

    /// <summary>
    /// Attempts to add data from the specified ZIP archive to the given EPUB book.
    /// </summary>
    /// <remarks>If the parsing operation returned a result with fatal errors, this function will return <see langword="false"/> and <paramref name="book"/> should not be changed.</remarks>
    /// <param name="book">The <see cref="EpubBook"/> instance to which data will be added. Cannot be null.</param>
    /// <param name="zip">The <see cref="ZipArchive"/> containing the data to be added to the book. Cannot be null.</param>
    /// <param name="minimumLogLevel">The minimum log level for logs.</param>
    /// <param name="logs">The logs produced by this parsing step.</param>
    /// <returns><see langword="true"/> if parsing did not result in a fatal error; otherwise, <see langword="false"/>.</returns>
    bool TryAddDataToBook(EpubBook book, ZipArchive zip, EpubLogLevel minimumLogLevel, out EpubLogCollectoin logs);
}