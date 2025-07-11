namespace DG.Epub.Logging;

/// <summary>
/// A log of an event that occured while trying to read an EPUB file.
/// </summary>
public class EpubLog
{
    private readonly EpubLogLevel _severity;
    private readonly string _message;

    /// <summary>
    /// The severity of this <see cref="EpubLog"/>.
    /// </summary>
    public EpubLogLevel Severity => _severity;

    /// <summary>
    /// The message describing this <see cref="EpubLog"/>.
    /// </summary>
    public string Message => _message;

    /// <summary>
    /// Initializes a new instance of <see cref="EpubLog"/>, with the given <paramref name="severity"/> and <paramref name="message"/>.
    /// </summary>
    /// <param name="severity"></param>
    /// <param name="message"></param>
    public EpubLog(EpubLogLevel severity, string message)
    {
        _severity = severity;
        _message = message ?? string.Empty;
    }

    /// <summary>
    /// Creates a log entry that is purely meant to be informational.
    /// </summary>
    /// <param name="message">The message describing the log event.</param>
    /// <returns>An <see cref="EpubLog"/> instance representing the informational log entry.</returns>
    public static EpubLog Informational(string message)
    {
        return new EpubLog(EpubLogLevel.Informational, message);
    }

    /// <summary>
    /// Creates a log entry that is meant to warn about possible problems.
    /// </summary>
    /// <param name="message">The message describing the log event.</param>
    /// <returns>An <see cref="EpubLog"/> instance representing the informational log entry.</returns>
    public static EpubLog Warning(string message)
    {
        return new EpubLog(EpubLogLevel.Warning, message);
    }

    /// <summary>
    /// Creates a log entry that represents an error during parsing.
    /// </summary>
    /// <param name="message">The message describing the log event.</param>
    /// <returns>An <see cref="EpubLog"/> instance representing the informational log entry.</returns>
    public static EpubLog Error(string message)
    {
        return new EpubLog(EpubLogLevel.Error, message);
    }

    /// <summary>
    /// Creates a log entry that represents a fatal error that makes further parsing impossible.
    /// </summary>
    /// <param name="message">The message describing the log event.</param>
    /// <returns>An <see cref="EpubLog"/> instance representing the informational log entry.</returns>
    public static EpubLog Fatal(string message)
    {
        return new EpubLog(EpubLogLevel.Fatal, message);
    }

    /// <summary>
    /// Returns a string representation of the current <see cref="EpubLog"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"[{_severity}] {_message}";
    }
}
