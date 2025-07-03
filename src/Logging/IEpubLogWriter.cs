using System.Collections.Generic;

namespace DG.Epub.Logging;

/// <summary>
/// Defines methods for writing and managing logs in the context of EPUB processing.
/// </summary>
public interface IEpubLogWriter
{
    /// <summary>
    /// Adds the given <paramref name="log"/> to the current log writer.
    /// </summary>
    /// <param name="log"></param>
    void Add(EpubLog log);

    /// <summary>
    /// Adds all of the given <paramref name="logs"/> to the current log writer.
    /// </summary>
    /// <param name="logs"></param>
    void AddAll(IEnumerable<EpubLog> logs);

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Debug"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    void AddDebug(string message);

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Informational"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    void AddInformational(string message);

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Warning"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    void AddWarning(string message);

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Error"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    void AddError(string message);

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Fatal"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    void AddFatal(string message);
}