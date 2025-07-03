using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DG.Epub.Logging;

/// <summary>
/// A collection of errors that occured during the reading of an EPUB file.
/// </summary>
public class EpubLogCollectoin : IEnumerable<EpubLog>
{
    private readonly EpubLogLevel _minimumLogLevel;
    private readonly List<EpubLog> _logs;
    private EpubLogLevel _highestSeverity = EpubLogLevel.Debug;

    /// <summary>
    /// Indicates the hightest <see cref="EpubLogLevel"/> of any <see cref="EpubLog"/> that has been added to this <see cref="EpubLogCollectoin"/>.
    /// </summary>
    public EpubLogLevel HighestSeverity => _highestSeverity;

    /// <summary>
    /// Initializes a new instance of <see cref="EpubLogCollectoin"/> without any logs.
    /// </summary>
    /// <param name="miminumLogLevel"></param>
    public EpubLogCollectoin(EpubLogLevel miminumLogLevel = EpubLogLevel.Informational)
    {
        _logs = new List<EpubLog>();
        _minimumLogLevel = miminumLogLevel;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EpubLogCollectoin"/> with the given <paramref name="log"/>.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="miminumLogLevel"></param>
    public EpubLogCollectoin(EpubLog log, EpubLogLevel miminumLogLevel = EpubLogLevel.Informational)
        : this(miminumLogLevel)
    {
        AddLog(log);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EpubLogCollectoin"/> class with the given <paramref name="logs"/>.
    /// </summary>
    /// <remarks>Logs in the <paramref name="logs"/> collection that do not meet the specified minimum log level will not be included in the new collection.</remarks>
    /// <param name="logs">A collection of <see cref="EpubLog"/> instances to be added to the log collection.</param>
    /// <param name="miminumLogLevel">The minimum log level to filter logs. Logs below this level will be ignored. Defaults to <see cref="EpubLogLevel.Informational"/>.</param>
    public EpubLogCollectoin(IEnumerable<EpubLog> logs, EpubLogLevel miminumLogLevel = EpubLogLevel.Informational)
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

    private void AddLog(EpubLog log)
    {
        if (log.Severity < _minimumLogLevel)
            return;

        OverwriteSeverityIfNeeded(log.Severity);
        _logs.Add(log);
    }

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Debug"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    public void AddDebug(string message)
    {
        AddLog(new EpubLog(EpubLogLevel.Debug, message));
    }

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Informational"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    public void AddInformational(string message)
    {
        AddLog(new EpubLog(EpubLogLevel.Informational, message));
    }

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Warning"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    public void AddWarning(string message)
    {
        AddLog(new EpubLog(EpubLogLevel.Warning, message));
    }

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Error"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    public void AddError(string message)
    {
        AddLog(new EpubLog(EpubLogLevel.Error, message));
    }

    /// <summary>
    /// Adds a new log with a severity of <see cref="EpubLogLevel.Fatal"/> and the given <paramref name="message"/>.
    /// </summary>
    /// <param name="message"></param>
    public void AddFatal(string message)
    {
        AddLog(new EpubLog(EpubLogLevel.Fatal, message));
    }

    /// <summary>
    /// Adds all of the given <paramref name="logs"/> to the current <see cref="EpubLogCollectoin"/>.
    /// </summary>
    /// <param name="logs"></param>
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
    public IEnumerator<EpubLog> GetEnumerator()
    {
        return ((IEnumerable<EpubLog>)_logs).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
