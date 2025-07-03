using DG.Epub.Logging;
using System.Collections.Generic;

namespace DG.Epub.Parsing;

/// <summary>
/// This class represents the result of parsing an EPUB file, including the parsed book object and any logs generated during the parsing process.
/// </summary>
public class EpubParsingResult<T>
{
    private readonly EpubLogCollectoin _logs;
    private readonly T? _value;

    /// <summary>
    /// Indicates the highest severity of any log that has been encounted while parsing the EPUB file.
    /// </summary>
    public EpubLogLevel MaxLogLevel => _logs.HighestSeverity;

    /// <summary>
    /// Gets a value indicating whether a fatal error has occurred, and parsing or checking of this EPUB file cannot continue.
    /// </summary>
    public bool HasFatalError => MaxLogLevel >= EpubLogLevel.Fatal;

    /// <summary>
    /// Gets the collection of logs generated during the parsing project.
    /// </summary>
    public IEnumerable<EpubLog> Logs => _logs;

    /// <summary>
    /// The result of an EPUB parsing action. Note that this can be <see langword="default"/>(<typeparamref name="T"/>) if <see cref="HasFatalError"/> is <see langword="true"/>.
    /// </summary>
    public T? Value => _value ?? default;

    /// <summary>
    /// Initializes a new instance of the <see cref="EpubParsingResult{T}"/> class with the specified <paramref name="value"/> and <paramref name="logs"/>.
    /// </summary>
    /// <param name="value">The parsed EPUB object.</param>
    /// <param name="logs">The collection of logs generated during the parsing process. If <see langword="null"/>, an empty log collection will be used.</param>
    public EpubParsingResult(T? value, EpubLogCollectoin? logs)
    {
        _logs = logs ?? new EpubLogCollectoin();
        _value = value;
    }

    /// <summary>
    /// Creates a new parsing result with the specified type, preserving the current log entries.
    /// </summary>
    /// <typeparam name="TOther">The type of the result value for the new parsing result.</typeparam>
    /// <returns>A new <see cref="EpubParsingResult{TOther}"/> instance containing a default value of type <typeparamref name="TOther"/>  and the log entries from the current parsing result.</returns>
    public EpubParsingResult<TOther> AsResultFor<TOther>()
    {
        return new EpubParsingResult<TOther>(default, _logs);
    }

    /// <summary>
    /// Copies the logs from the current parsing result to the specified log collection.
    /// </summary>
    /// <param name="logCollection">The target collection to which logs will be copied. Cannot be null.</param>
    /// <returns>The current <see cref="EpubParsingResult{T}"/> instance, allowing for method chaining.</returns>
    public EpubParsingResult<T> AndCopyLogsTo(EpubLogCollectoin logCollection)
    {
        logCollection.AddAll(_logs);
        return this;
    }

    public bool TryGetValue(out T? value)
    {
        if (HasFatalError)
        {
            value = default;
            return false;
        }
        value = _value;
        return true;
    }
}

/// <summary>
/// Provides static methods for creating instances of <see cref="EpubParsingResult{T}"/> to represent the results of parsing EPUB files.
/// </summary>
/// <remarks>This class includes methods for creating successful parsing results, allowing users to specify the parsed value and any logs generated during the parsing process.</remarks>
public static class EPubParsingResult
{
    /// <summary>
    /// Creates a parsing result with the specified value and logs.
    /// </summary>
    /// <typeparam name="T">The type of the parsed value.</typeparam>
    /// <param name="value">The parsed EPUB object.</param>
    /// <param name="logs">The collection of logs generated during the parsing process. If <see langword="null"/>, an empty log collection will be used.</param>
    /// <returns>An <see cref="EpubParsingResult{T}"/> instance representing a completed parsing result.</returns>
    public static EpubParsingResult<T> Completed<T>(T value, EpubLogCollectoin logs)
    {
        return new EpubParsingResult<T>(value, logs);
    }

    /// <summary>
    /// Creates a fatal error parsing result with the given <paramref name="logs"/> leading up to the error.
    /// </summary>
    /// <param name="logs"></param>
    /// <returns>An <see cref="EpubParsingResult{T}"/> instance representing a fatal parsing error, with no parsed value and the provided <paramref name="logs"/>.</returns>
    public static EpubParsingResult<T> FatalFor<T>(EpubLogCollectoin logs)
    {
        return new EpubParsingResult<T>(default, logs);
    }

    /// <summary>
    /// Creates a fatal error parsing result with the specified error message.
    /// </summary>
    /// <param name="message">The error message describing the fatal condition.</param>
    /// <param name="minimumLogLevel"></param>
    /// <returns>An <see cref="EpubParsingResult{T}"/> instance representing a fatal parsing error, with no parsed value and the provided error message.</returns>
    public static EpubParsingResult<T> FatalFor<T>(string message, EpubLogLevel minimumLogLevel = EpubLogLevel.Informational)
    {
        return new EpubParsingResult<T>(default, new EpubLogCollectoin(EpubLog.Fatal(message), minimumLogLevel));
    }
}