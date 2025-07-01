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
        private readonly List<EPubLog> _logs = new List<EPubLog>();
        private EPubLogSeverity _highestSeverity = EPubLogSeverity.Informational;

        /// <summary>
        /// Indicates the hightest <see cref="EPubLogSeverity"/> of any <see cref="EPubLog"/> that has been added to this <see cref="EPubLogCollection"/>.
        /// </summary>
        public EPubLogSeverity HighestSeverity => _highestSeverity;

        /// <summary>
        /// Initializes a new instance of <see cref="EPubLogCollection"/> without any logs.
        /// </summary>
        public EPubLogCollection()
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EPubLogCollection"/> with the given <paramref name="log"/>.
        /// </summary>
        /// <param name="log"></param>
        public EPubLogCollection(EPubLog log)
        {
            AddLog(log);
        }

        /// <summary>
        /// Returns a new instance of <see cref="EPubLogCollection"/> without any logs.
        /// </summary>
        public static EPubLogCollection Empty => new EPubLogCollection();

        /// <summary>
        /// Returns a new instance of <see cref="EPubLogCollection"/> containing a single log with a severity of <see cref="EPubLogSeverity.Informational"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static EPubLogCollection ForInformational(string message)
        {
            return new EPubLogCollection(new EPubLog(EPubLogSeverity.Informational, message));
        }

        /// <summary>
        /// Returns a new instance of <see cref="EPubLogCollection"/> containing a single log with a severity of <see cref="EPubLogSeverity.Warning"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static EPubLogCollection ForWarning(string message)
        {
            return new EPubLogCollection(new EPubLog(EPubLogSeverity.Warning, message));
        }

        /// <summary>
        /// Returns a new instance of <see cref="EPubLogCollection"/> containing a single log with a severity of <see cref="EPubLogSeverity.Error"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static EPubLogCollection ForError(string message)
        {
            return new EPubLogCollection(new EPubLog(EPubLogSeverity.Error, message));
        }

        /// <summary>
        /// Returns a new instance of <see cref="EPubLogCollection"/> containing a single log with a severity of <see cref="EPubLogSeverity.Fatal"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static EPubLogCollection ForFatal(string message)
        {
            return new EPubLogCollection(new EPubLog(EPubLogSeverity.Fatal, message));
        }

        private void OverwriteSeverityIfNeeded(EPubLogSeverity severity)
        {
            if (severity > _highestSeverity)
            {
                _highestSeverity = severity;
            }
        }

        private void AddLog(EPubLog log)
        {
            OverwriteSeverityIfNeeded(log.Severity);
            _logs.Add(log);
        }

        /// <summary>
        /// Adds a new log with a severity of <see cref="EPubLogSeverity.Informational"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public void AddInformational(string message)
        {
            AddLog(new EPubLog(EPubLogSeverity.Informational, message));
        }

        /// <summary>
        /// Adds a new log with a severity of <see cref="EPubLogSeverity.Warning"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public void AddWarning(string message)
        {
            AddLog(new EPubLog(EPubLogSeverity.Warning, message));
        }

        /// <summary>
        /// Adds a new log with a severity of <see cref="EPubLogSeverity.Error"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public void AddError(string message)
        {
            AddLog(new EPubLog(EPubLogSeverity.Error, message));
        }

        /// <summary>
        /// Adds a new log with a severity of <see cref="EPubLogSeverity.Fatal"/> and the given <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public void AddFatal(string message)
        {
            AddLog(new EPubLog(EPubLogSeverity.Fatal, message));
        }

        /// <summary>
        /// Adds all logs in the given <paramref name="logs"/> to the current <see cref="EPubLogCollection"/>.
        /// </summary>
        /// <param name="logs"></param>
        public void AddAll(IEnumerable<EPubLog> logs)
        {
            if (!logs.Any())
            {
                return;
            }

            OverwriteSeverityIfNeeded(logs.Max(l => l.Severity));
            _logs.AddRange(logs);
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
