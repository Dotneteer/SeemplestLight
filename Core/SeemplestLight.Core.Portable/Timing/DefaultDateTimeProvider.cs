using System;

namespace SeemplestLight.Core.Portable.Timing
{
    /// <summary>
    /// This class defines a date time provider that apps can use in production mode
    /// </summary>
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date and time
        /// </summary>
        /// <param name="kind">Kind of DateTime</param>
        /// <returns>The current date and time</returns>
        public DateTime GetCurrenDateTime(DateTimeKind kind = DateTimeKind.Local)
        {
            return kind == DateTimeKind.Local ? DateTime.Now : DateTime.UtcNow;
        }
    }
}