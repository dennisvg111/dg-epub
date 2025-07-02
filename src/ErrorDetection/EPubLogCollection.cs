using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DG.Epub.ErrorDetection
{
    /// <summary>
    /// A collection of errors that occured during the reading of an ePub file.
    /// </summary>
    public class EPubLogCollection : IEnumerable<EPubLog>
    {
        private readonly EPubLogLevel _minimumLogLevel;
        private readonly List<EPubLog> _logs;
        private EPubLogLevel _highestSeverity = EPubLogLevel.Debug;

        /// <summary>
        /// Indicates the hightest <see cref="EPubLogLevel"/> of any <see cref="EPubLog"/> that has been added to this <see cref="EPubLogCollection"/>.
        /// </summary>
        public EPubLogLevel HighestSeverity => _highestSeverity;

        /// <summary>
        /// Initializes a new instance of <see cref="EPubLogCollection"/> without any logs.
        /// </summary>
        /// <param name="miminumLogLevel"></param>
        public EPubLogCollection(EPubLogLevel miminumLogLevel = EPubLogLevel.Informational)
        {
            _logs = new List<EPubLog>();
            _minimumLogLevel = miminumLogLevel;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="EPubLogCollection"/> with the given <paramref name="log"/>.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="miminumLogLevel"></param>
        public EPubLogCollection(EPubLog log, EPubLogLevel miminumLogLevel = EPubLogLevel.Informational)
            : this(miminumLogLevel)
        {
            AddLog(log);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EPubLogCollection"/> class with the given <paramref name="logs"/>.
        /// </summary>
        /// <remarks>Logs in the <paramref name="logs"/> collection that do not meet the specified minimum log level will not be included in the new collection.</remarks>
        /// <param name="logs">A collection of <see cref="EPubLog"/> instances to be added to the log collection.</param>
        /// <param name="miminumLogLevel">The minimum log level to filter logs. Logs below this level will be ignored. Defaults to <see cref="EPubLogLevel.Informational"/>.</param>
        public EPubLogCollection(IEnumerable<EPubLog> logs, EPubLogLevel miminumLogLevel = EPubLogLevel.Informational)
            : this(miminumLogLevel)
        {
            AddAll(logs);
        }

        private void OverwriteSeverityIfNeeded(EPubLogLevel severity)
        {
            if (severity > _highestSeverity)
            {
                _highestSeverity = severity;
            }
        }

        private void AddLog(EPubLog log)
        {
            if (log.Severity < _minimumLogLevel)
                return;

            OverwriteSeverityIfNeeded(log.Severity);
            _logs.Add(log);
        }

        /// <summary>
        /// Adds a new log with a severity of <see cref="EPubLogLevel.Debug"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public void AddDebug(string message)
        {
            AddLog(new EPubLog(EPubLogLevel.Debug, message));
        }

        /// <summary>
        /// Adds a new log with a severity of <see cref="EPubLogLevel.Informational"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public void AddInformational(string message)
        {
            AddLog(new EPubLog(EPubLogLevel.Informational, message));
        }

        /// <summary>
        /// Adds a new log with a severity of <see cref="EPubLogLevel.Warning"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public void AddWarning(string message)
        {
            AddLog(new EPubLog(EPubLogLevel.Warning, message));
        }

        /// <summary>
        /// Adds a new log with a severity of <see cref="EPubLogLevel.Error"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public void AddError(string message)
        {
            AddLog(new EPubLog(EPubLogLevel.Error, message));
        }

        /// <summary>
        /// Adds a new log with a severity of <see cref="EPubLogLevel.Fatal"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public void AddFatal(string message)
        {
            AddLog(new EPubLog(EPubLogLevel.Fatal, message));
        }

        /// <summary>
        /// Adds all of the given <paramref name="logs"/> to the current <see cref="EPubLogCollection"/>.
        /// </summary>
        /// <param name="logs"></param>
        public void AddAll(IEnumerable<EPubLog> logs)
        {
            logs = logs.Where(l => l.Severity >= _minimumLogLevel);
            if (!logs.Any())
            {
                return;
            }

            OverwriteSeverityIfNeeded(logs.Max(l => l.Severity));
            _logs.AddRange(logs);
        }

        /// <summary>
        /// Creates a new <see cref="EPubLogCollection"/> instance with the specified <see cref="EPubLogLevel.Debug"/> log added.
        /// </summary>
        /// <param name="message">The log message to add to the new log collection.</param>
        /// <returns>A new <see cref="EPubLogCollection"/> instance containing the current log entries and the new <paramref name="message"/>.</returns>
        public EPubLogCollection WithDebug(string message)
        {
            var copy = new EPubLogCollection(this, _minimumLogLevel);
            copy.AddDebug(message);
            return copy;
        }

        /// <summary>
        /// Creates a new <see cref="EPubLogCollection"/> instance with the specified <see cref="EPubLogLevel.Informational"/> log added.
        /// </summary>
        /// <param name="message">The log message to add to the new log collection.</param>
        /// <returns>A new <see cref="EPubLogCollection"/> instance containing the current log entries and the new <paramref name="message"/>.</returns>
        public EPubLogCollection WithInformational(string message)
        {
            var copy = new EPubLogCollection(this, _minimumLogLevel);
            copy.AddInformational(message);
            return copy;
        }

        /// <summary>
        /// Creates a new <see cref="EPubLogCollection"/> instance with the specified <see cref="EPubLogLevel.Warning"/> log added.
        /// </summary>
        /// <param name="message">The log message to add to the new log collection.</param>
        /// <returns>A new <see cref="EPubLogCollection"/> instance containing the current log entries and the new <paramref name="message"/>.</returns>
        public EPubLogCollection WithWarning(string message)
        {
            var copy = new EPubLogCollection(this, _minimumLogLevel);
            copy.AddWarning(message);
            return copy;
        }

        /// <summary>
        /// Creates a new <see cref="EPubLogCollection"/> instance with the specified <see cref="EPubLogLevel.Error"/> log added.
        /// </summary>
        /// <param name="message">The log message to add to the new log collection.</param>
        /// <returns>A new <see cref="EPubLogCollection"/> instance containing the current log entries and the new <paramref name="message"/>.</returns>
        public EPubLogCollection WithError(string message)
        {
            var copy = new EPubLogCollection(this, _minimumLogLevel);
            copy.AddError(message);
            return copy;
        }

        /// <summary>
        /// Creates a new <see cref="EPubLogCollection"/> instance with the specified <see cref="EPubLogLevel.Fatal"/> log added.
        /// </summary>
        /// <param name="message">The log message to add to the new log collection.</param>
        /// <returns>A new <see cref="EPubLogCollection"/> instance containing the current log entries and the new <paramref name="message"/>.</returns>
        public EPubLogCollection WithFatal(string message)
        {
            var copy = new EPubLogCollection(this, _minimumLogLevel);
            copy.AddFatal(message);
            return copy;
        }

        /// <inheritdoc/>
        public IEnumerator<EPubLog> GetEnumerator()
        {
            return ((IEnumerable<EPubLog>)_logs).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
