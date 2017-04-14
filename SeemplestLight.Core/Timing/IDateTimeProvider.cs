using System;

namespace SeemplestLight.Core.Timing
{
    /// <summary>
    /// This interface defines operations that can be used to obtain the 
    /// current date and time
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date and time
        /// </summary>
        /// <param name="kind">Kind of DateTime</param>
        /// <returns>The current date and time</returns>
        DateTime GetCurrenDateTime(DateTimeKind kind = DateTimeKind.Local);
    }
}