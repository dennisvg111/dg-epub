namespace DG.Epub.Logging;

/// <summary>
/// The level or severity of an <see cref="EpubLog"/>.
/// </summary>
public enum EpubLogLevel
{
    /// <summary>
    /// Developer-oriented debug information about the parsing process.
    /// </summary>
    Debug = 0,

    /// <summary>
    /// Any information about an EPUB file that is not a warning or error, and not specifically debugging information for developers.
    /// </summary>
    Informational = 1,

    /// <summary>
    /// An aspect of the book is not compliant with the EPUB specification, but the book is likely to be renderable by most reading systems.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// A major problem that will almost certainly prevent successful EPUB rendering.
    /// </summary>
    Error = 3,

    /// <summary>
    /// A severe problem with the EPUB file itself prevents further checking/reading.
    /// </summary>
    Fatal = 4
}
