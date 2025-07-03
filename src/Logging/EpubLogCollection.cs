using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DG.Epub.Logging;

/// <summary>
/// A collection of errors that occured during the reading of an EPUB file.
/// </summary>
public class EpubLogCollection : IReadOnlyEpubLogCollection, IEpubLogWriter
{
    private readonly EpubLogLevel _minimumLogLevel;
    private readonly List<EpubLog> _logs;
    private EpubLogLevel _highestSeverity = EpubLogLevel.Debug;

    /// <inheritdoc/>
    public EpubLogLevel HighestSeverity => _highestSeverity;

    /// <inheritdoc/>
    public bool ContainsFatalError => HighestSeverity >= EpubLogLevel.Fatal; // use greater than or equals so future levels above fatal are also included.

    /// <summary>
    /// Initializes a new instance of <see cref="EpubLogCollection"/> without any logs.
    /// </summary>
    /// <param name="miminumLogLevel"></param>
    public EpubLogCollection(EpubLogLevel miminumLogLevel = EpubLogLevel.Informational)
    {
        _logs = new List<EpubLog>();
        _minimumLogLevel = miminumLogLevel;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EpubLogCollection"/> with the given <paramref name="log"/>.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="miminumLogLevel"></param>
    public EpubLogCollection(EpubLog log, EpubLogLevel miminumLogLevel = EpubLogLevel.Informational)
        : this(miminumLogLevel)
    {
        Add(log);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EpubLogCollection"/> class with the given <paramref name="logs"/>.
    /// </summary>
    /// <remarks>Logs in the <paramref name="logs"/> collection that do not meet the specified minimum log level will not be included in the new collection.</remarks>
    /// <param name="logs">A collection of <see cref="EpubLog"/> instances to be added to the log collection.</param>
    /// <param name="miminumLogLevel">The minimum log level to filter logs. Logs below this level will be ignored. Defaults to <see cref="EpubLogLevel.Informational"/>.</param>
    public EpubLogCollection(IEnumerable<EpubLog> logs, EpubLogLevel miminumLogLevel = EpubLogLevel.Informational)
        : this(miminumLogLevel)
    {
        AddAll(logs);
    }

    private void OverwriteSeverityIfNeeded(EpubLogLevel severity)
    {
        if (severity > _highestSeverity)
        {
            _highestSeverity = severity;
        }
    }

    /// <inheritdoc/>
    public void Add(EpubLog log)
    {
        if (log.Severity < _minimumLogLevel)
            return;

        OverwriteSeverityIfNeeded(log.Severity);
        _logs.Add(log);
    }

    /// <inheritdoc/>
    public void AddAll(IEnumerable<EpubLog> logs)
    {
        logs = logs.Where(l => l.Severity >= _minimumLogLevel);
        if (!logs.Any())
        {
            return;
        }

        OverwriteSeverityIfNeeded(logs.Max(l => l.Severity));
        _logs.AddRange(logs);
    }

    /// <inheritdoc/>
    public void AddDebug(string message)
    {
        Add(new EpubLog(EpubLogLevel.Debug, message));
    }

    /// <inheritdoc/>
    public void AddInformational(string message)
    {
        Add(new EpubLog(EpubLogLevel.Informational, message));
    }

    /// <inheritdoc/>
    public void AddWarning(string message)
    {
        Add(new EpubLog(EpubLogLevel.Warning, message));
    }

    /// <inheritdoc/>
    public void AddError(string message)
    {
        Add(new EpubLog(EpubLogLevel.Error, message));
    }

    /// <inheritdoc/>
    public void AddFatal(string message)
    {
        Add(new EpubLog(EpubLogLevel.Fatal, message));
    }

    /// <inheritdoc/>
    public IEnumerator<EpubLog> GetEnumerator()
    {
        return ((IEnumerable<EpubLog>)_logs).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
