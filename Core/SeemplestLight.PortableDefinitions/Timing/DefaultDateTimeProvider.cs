using System;

namespace SeemplestLight.PortableCore.Timing
{
    /// <summary>
    /// This class defines a date time provider that apps can use in production mode
    /// </summary>
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date and time (UTC)
        /// </summary>
        /// <returns>The current date and time (UTC)</returns>
        public DateTime GetCurrenDateTimeUtc()
        {
            return DateTime.UtcNow;
        }
    }
}