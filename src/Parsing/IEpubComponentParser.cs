using DG.Epub.Logging;
using System.IO.Compression;

namespace DG.Epub.Parsing;

/// <summary>
/// Defines methods for parsing EPUB components and integrating them into an EPUB book.
/// </summary>
/// <typeparam name="TComponent">The type of the EPUB component to be parsed.</typeparam>
public interface IEpubComponentParser<TComponent>
{
    /// <summary>
    /// Attempts to parse the specified ZIP archive and extract the component data.
    /// </summary>
    /// <remarks>This method does not throw exceptions for parsing errors. Instead, it logs errors to the provided <paramref name="logs"/> collection and returns <see langword="false"/> if parsing fails.</remarks>
    /// <param name="zip">The ZIP archive containing the data to be parsed. This will never be <see langword="null"/>.</param>
    /// <param name="logs">A collection to store log entries generated during the parsing process. This will never be <see langword="null"/>..</param>
    /// <param name="data">
    /// <para>When this method returns, contains the parsed component data</para>
    /// <para>Note that this can still be <see langword="null"/> even if the return value of this function is <see langword="true"/> if the component was not found but is optional.</para>
    /// </param>
    /// <returns><see langword="true"/> if parsing of this EPUB file can continue; otherwise, <see langword="false"/>.</returns>
    bool TryParse(ZipArchive zip, EpubLogCollectoin logs, out TComponent? data);

    /// <summary>
    /// Adds the specified parsed data to the given EPUB book.
    /// </summary>
    /// <remarks>This method modifies the state of the provided <paramref name="book"/> by incorporating the specified <paramref name="data"/>.</remarks>
    /// <param name="book">The EPUB book to which the parsed data will be added. This should never be <see langword="null"/>.</param>
    /// <param name="data">The parsed data to add to the book. Can be <see langword="null"/> for some optional components if no data was found while parsing.</param>
    void AddToBook(EpubBook book, TComponent? data);
}