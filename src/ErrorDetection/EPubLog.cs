namespace DG.Epub.ErrorDetection
{
    /// <summary>
    /// A resulting warning or error that occured while trying to read an ePub file.
    /// </summary>
    public class EPubLog
    {
        private readonly EPubLogSeverity _severity;
        private readonly string _message;

        /// <summary>
        /// The severity of this <see cref="EPubLog"/>.
        /// </summary>
        public EPubLogSeverity Severity => _severity;

        /// <summary>
        /// The message describing this <see cref="EPubLog"/>.
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// Initializes a new instance of <see cref="EPubLog"/>, with the given <paramref name="severity"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        public EPubLog(EPubLogSeverity severity, string message)
        {
            _severity = severity;
            _message = message;
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
