using System;

namespace SeemplestLight.Core.Timing
{
    /// <summary>
    /// This class defines the singleton object used within the app to represent
    /// DateTime information
    /// </summary>
    public static class DateTimeProvider
    {
        private static IDateTimeProvider s_Provider;

        static DateTimeProvider()
        {
            Reset();
        }

        /// <summary>
        /// Resets the class to use DefaultDateTimeProvider
        /// </summary>
        public static void Reset()
        {
            s_Provider = new DefaultDateTimeProvider();
        }

        /// <summary>
        /// Sets the environment provider to the specified one
        /// </summary>
        /// <param name="provider">DateTimeProvider to use</param>
        public static void SetProvider(IDateTimeProvider provider)
        {
            s_Provider = provider;
        }

        /// <summary>
        /// Gets the current date and time
        /// </summary>
        /// <param name="kind">Kind of DateTime</param>
        /// <returns>The current date and time</returns>
        public static DateTime GetCurrenDateTime(DateTimeKind kind = DateTimeKind.Local)
        {
            return s_Provider.GetCurrenDateTime(kind);
        }
    }
}