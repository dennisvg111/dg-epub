namespace DG.Epub.ErrorDetection
{
    /// <summary>
    /// This class represents the result of parsing an ePub file, including the parsed book and any logs generated during the parsing process.
    /// </summary>
    public class EPubParsingResult<T>
    {
        private readonly EPubLogCollection _logs;
        private readonly T _value;

        /// <summary>
        /// Indicates the highest severity of any log that has been encounted while parsing the ePub file.
        /// </summary>
        public EPubLogSeverity ErrorSeverity => _logs.HighestSeverity;

        /// <summary>
        /// Gets a value indicating whether a fatal error has occurred, and parsing or checking of this ePub file cannot continue.
        /// </summary>
        public bool HasFatalError => ErrorSeverity >= EPubLogSeverity.Fatal;

        /// <summary>
        /// The result of an ePub parsing action. Not that this can be <see langword="null"/> if a fatal error occured.
        /// </summary>
        public T Value => _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EPubParsingResult{T}"/> class with the specified <paramref name="value"/> and <paramref name="logs"/>.
        /// </summary>
        /// <param name="value">The parsed ePub object.</param>
        /// <param name="logs">The collection of logs generated during the parsing process. If <see langword="null"/>, an empty log collection will be used.</param>
        public EPubParsingResult(T value, EPubLogCollection logs)
        {
            _logs = logs ?? EPubLogCollection.Empty;
            _value = value;
        }

        /// <summary>
        /// Creates a fatal error parsing result with the specified error message.
        /// </summary>
        /// <param name="message">The error message describing the fatal condition.</param>
        /// <returns>An <see cref="EPubParsingResult{T}"/> instance representing a fatal parsing error, with no parsed value and the provided error message.</returns>
        public static EPubParsingResult<T> Fatal(string message)
        {
            return new EPubParsingResult<T>(default(T), EPubLogCollection.ForFatal(message));
        }
    }

    /// <summary>
    /// Provides factory methods for creating instances of <see cref="EPubParsingResult{T}"/>  /// to represent the results of parsing ePub files.
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
        public static EPubParsingResult<T> Success<T>(T value, EPubLogCollection logs = null)
        {
            return new EPubParsingResult<T>(value, logs);
        }
    }
}