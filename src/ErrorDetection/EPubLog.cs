namespace DG.Epub.ErrorDetection
{
    /// <summary>
    /// A log of an event that occured while trying to read an ePub file.
    /// </summary>
    public class EPubLog
    {
        private readonly EPubLogLevel _severity;
        private readonly string _message;

        /// <summary>
        /// The severity of this <see cref="EPubLog"/>.
        /// </summary>
        public EPubLogLevel Severity => _severity;

        /// <summary>
        /// The message describing this <see cref="EPubLog"/>.
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// Initializes a new instance of <see cref="EPubLog"/>, with the given <paramref name="severity"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        public EPubLog(EPubLogLevel severity, string message)
        {
            _severity = severity;
            _message = message ?? string.Empty;
        }

        /// <summary>
        /// Creates a log entry that is purely meant to be informational.
        /// </summary>
        /// <param name="message">The message describing the log event.</param>
        /// <returns>An <see cref="EPubLog"/> instance representing the informational log entry.</returns>
        public static EPubLog Informational(string message)
        {
            return new EPubLog(EPubLogLevel.Informational, message);
        }

        /// <summary>
        /// Creates a log entry that is meant to warn about possible problems.
        /// </summary>
        /// <param name="message">The message describing the log event.</param>
        /// <returns>An <see cref="EPubLog"/> instance representing the informational log entry.</returns>
        public static EPubLog Warning(string message)
        {
            return new EPubLog(EPubLogLevel.Warning, message);
        }

        /// <summary>
        /// Creates a log entry that represents an error during parsing.
        /// </summary>
        /// <param name="message">The message describing the log event.</param>
        /// <returns>An <see cref="EPubLog"/> instance representing the informational log entry.</returns>
        public static EPubLog Error(string message)
        {
            return new EPubLog(EPubLogLevel.Error, message);
        }

        /// <summary>
        /// Creates a log entry that represents a fatal error that makes further parsing impossible.
        /// </summary>
        /// <param name="message">The message describing the log event.</param>
        /// <returns>An <see cref="EPubLog"/> instance representing the informational log entry.</returns>
        public static EPubLog Fatal(string message)
        {
            return new EPubLog(EPubLogLevel.Fatal, message);
        }

        /// <summary>
        /// Returns a string representation of the current <see cref="EPubLog"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{_severity}] {_message}";
        }
    }
}
