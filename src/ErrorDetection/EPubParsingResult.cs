using System.Collections.Generic;

namespace DG.Epub.ErrorDetection
{
    /// <summary>
    /// This class represents the result of parsing an ePub file, including the parsed book object and any logs generated during the parsing process.
    /// </summary>
    public class EPubParsingResult<T>
    {
        private readonly EPubLogCollection _logs;
        private readonly T? _value;

        /// <summary>
        /// Indicates the highest severity of any log that has been encounted while parsing the ePub file.
        /// </summary>
        public EPubLogLevel MaxLogLevel => _logs.HighestSeverity;

        /// <summary>
        /// Gets a value indicating whether a fatal error has occurred, and parsing or checking of this ePub file cannot continue.
        /// </summary>
        public bool HasFatalError => MaxLogLevel >= EPubLogLevel.Fatal;

        /// <summary>
        /// Gets the collection of logs generated during the parsing project.
        /// </summary>
        public IEnumerable<EPubLog> Logs => _logs;

        /// <summary>
        /// The result of an ePub parsing action. Note that this can be <see langword="default"/>(<typeparamref name="T"/>) if <see cref="HasFatalError"/> is <see langword="true"/>.
        /// </summary>
        public T? Value => _value ?? default(T);

        /// <summary>
        /// Initializes a new instance of the <see cref="EPubParsingResult{T}"/> class with the specified <paramref name="value"/> and <paramref name="logs"/>.
        /// </summary>
        /// <param name="value">The parsed ePub object.</param>
        /// <param name="logs">The collection of logs generated during the parsing process. If <see langword="null"/>, an empty log collection will be used.</param>
        public EPubParsingResult(T? value, EPubLogCollection? logs)
        {
            _logs = logs ?? new EPubLogCollection();
            _value = value;
        }

        /// <summary>
        /// Creates a new parsing result with the specified type, preserving the current log entries.
        /// </summary>
        /// <typeparam name="TOther">The type of the result value for the new parsing result.</typeparam>
        /// <returns>A new <see cref="EPubParsingResult{TOther}"/> instance containing a default value of type <typeparamref name="TOther"/>  and the log entries from the current parsing result.</returns>
        public EPubParsingResult<TOther> AsResultFor<TOther>()
        {
            return new EPubParsingResult<TOther>(default(TOther?), _logs);
        }

        /// <summary>
        /// Copies the logs from the current parsing result to the specified log collection.
        /// </summary>
        /// <param name="logCollection">The target collection to which logs will be copied. Cannot be null.</param>
        /// <returns>The current <see cref="EPubParsingResult{T}"/> instance, allowing for method chaining.</returns>
        public EPubParsingResult<T> AndCopyLogsTo(EPubLogCollection logCollection)
        {
            logCollection.AddAll(_logs);
            return this;
        }

        public bool TryGetValue(out T? value)
        {
            if (HasFatalError)
            {
                value = default(T?);
                return false;
            }
            value = _value;
            return true;
        }
    }

    /// <summary>
    /// Provides static methods for creating instances of <see cref="EPubParsingResult{T}"/> to represent the results of parsing ePub files.
    /// </summary>
    /// <remarks>This class includes methods for creating successful parsing results, allowing users to specify the parsed value and any logs generated during the parsing process.</remarks>
    public static class EPubParsingResult
    {
        /// <summary>
        /// Creates a successful parsing result with the specified value and logs.
        /// </summary>
        /// <typeparam name="T">The type of the parsed value.</typeparam>
        /// <param name="value">The parsed ePub object.</param>
        /// <param name="logs">The collection of logs generated during the parsing process. If <see langword="null"/>, an empty log collection will be used.</param>
        /// <returns>An <see cref="EPubParsingResult{T}"/> instance representing a successful parsing result.</returns>
        public static EPubParsingResult<T> Success<T>(T value, EPubLogCollection logs)
        {
            return new EPubParsingResult<T>(value, logs);
        }

        /// <summary>
        /// Creates a fatal error parsing result with the given <paramref name="logs"/> leading up to the error.
        /// </summary>
        /// <param name="logs"></param>
        /// <returns>An <see cref="EPubParsingResult{T}"/> instance representing a fatal parsing error, with no parsed value and the provided <paramref name="logs"/>.</returns>
        public static EPubParsingResult<T> FatalFor<T>(EPubLogCollection logs)
        {
            return new EPubParsingResult<T>(default(T?), logs);
        }

        /// <summary>
        /// Creates a fatal error parsing result with the specified error message.
        /// </summary>
        /// <param name="message">The error message describing the fatal condition.</param>
        /// <param name="previousLogs"></param>
        /// <returns>An <see cref="EPubParsingResult{T}"/> instance representing a fatal parsing error, with no parsed value and the provided error message.</returns>
        public static EPubParsingResult<T> FatalFor<T>(string message, EPubLogCollection previousLogs)
        {
            return new EPubParsingResult<T>(default(T?), previousLogs.WithFatal(message));
        }

        /// <summary>
        /// Creates a fatal error parsing result with the specified error message.
        /// </summary>
        /// <param name="message">The error message describing the fatal condition.</param>
        /// <param name="minimumLogLevel"></param>
        /// <returns>An <see cref="EPubParsingResult{T}"/> instance representing a fatal parsing error, with no parsed value and the provided error message.</returns>
        public static EPubParsingResult<T> FatalFor<T>(string message, EPubLogLevel minimumLogLevel = EPubLogLevel.Informational)
        {
            return new EPubParsingResult<T>(default(T?), new EPubLogCollection(EPubLog.Fatal(message), minimumLogLevel));
        }
    }
}