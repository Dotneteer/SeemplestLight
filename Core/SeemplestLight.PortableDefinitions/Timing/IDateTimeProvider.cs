using System;

namespace SeemplestLight.PortableCore.Timing
{
    /// <summary>
    /// This interface defines operations that can be used to obtain the 
    /// current date and time
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date and time (UTC)
        /// </summary>
        /// <returns>The current date and time (UTC)</returns>
        DateTime GetCurrenDateTimeUtc();
    }
}