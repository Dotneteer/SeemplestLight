namespace SeemplestLight.Core.Portable.Diagnostics
{
    /// <summary>
    /// This class provides a base class for trace entry loggers
    /// </summary>
    public abstract class TraceEntryLoggerBase: ITraceEntryLogger
    {
        /// <summary>
        /// Logs the specified trace entry
        /// </summary>
        /// <param name="item">Trace entry</param>
        public void Log(TraceEntryBase item)
        {
            item.EnsureProperties();
            DoLog(item);
        }

        /// <summary>
        /// Override to specify how the trace entry should be logged.
        /// </summary>
        /// <param name="item">Trace entry</param>
        protected abstract void DoLog(TraceEntryBase item);

        public void Log(TraceEntryType type, string operationType, string message, string detailedMessage = null)
        {
            var item = new TraceEntryBase
            {
                Type = type,
                OperationType = operationType,
                Message = message,
                DetailedMessage = detailedMessage
            };
            Log(item);
        }

        /// <summary>
        /// Logs an informational entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogInfo(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceEntryType.Informational, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs a success entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogSuccess(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceEntryType.Success, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs a warning entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogWarning(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceEntryType.Warning, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs an error entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogError(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceEntryType.Error, operationType, message, detailedMessage);
        }

        /// <summary>
        /// Logs a fatal error entry with the specified operation type, message, and details.
        /// </summary>
        /// <param name="operationType">Type of operation logged</param>
        /// <param name="message">Log message</param>
        /// <param name="detailedMessage">Detailed log message</param>
        public void LogFatalError(string operationType, string message, string detailedMessage = null)
        {
            Log(TraceEntryType.Fatal, operationType, message, detailedMessage);
        }
    }
}