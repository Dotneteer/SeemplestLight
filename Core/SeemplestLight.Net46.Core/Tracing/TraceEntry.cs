using SeemplestLight.Net46.Core.Configuration;
using SeemplestLight.PortableCore.Timing;
using SeemplestLight.PortableCore.Tracing;
using System.Threading;

namespace SeemplestLight.Net46.Core.Tracing
{
    /// <summary>
    /// This class defines a trace log item
    /// </summary>
    public class TraceEntry: TraceEntryBase
    {
        /// <summary>
        /// Fills up properties that are not defined explicitly.
        /// </summary>
        public override void EnsureProperties()
        {
            // --- Provide a timestamp
            if (!TimestampUtc.HasValue)
            {
                TimestampUtc = DateTimeProvider.GetCurrenDateTimeUtc();
            }

            // --- Provide the current machine's name as server name
            if (HostName == null)
            {
                HostName = EnvironmentProvider.GetHostName();
            }

            // --- Provide thread information
            if (!ThreadId.HasValue)
            {
                ThreadId = Thread.CurrentThread.ManagedThreadId;
            }
        }
    }
}