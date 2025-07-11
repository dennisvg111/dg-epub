using System.Collections.Generic;

namespace DG.Epub.Logging;

/// <summary>
/// Represents a read-only collection of <see cref="EpubLog"/> entries.
/// Provides information about the presence of fatal errors and the highest severity level in the collection.
/// </summary>
public interface IReadOnlyEpubLogCollection : IEnumerable<EpubLog>
{
    /// <summary>
    /// Gets a value indicating whether any log in this collection has a level that means parsing or checking of this EPUB file cannot continue.
    /// </summary>
    bool ContainsFatalError { get; }

    /// <summary>
    /// Gets the highest log severity present in the collection.
    /// </summary>
    EpubLogLevel HighestSeverity { get; }
}