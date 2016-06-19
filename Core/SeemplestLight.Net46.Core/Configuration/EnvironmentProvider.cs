using SeemplestLight.PortableCore.Configuration;

namespace SeemplestLight.Net46.Core.Configuration
{
    /// <summary>
    /// This class defines the singleton object used within the app to represent
    /// environment information
    /// </summary>
    public static class EnvironmentProvider
    {
        private static IEnvironmentProvider s_Provider;

        static EnvironmentProvider()
        {
            Reset();
        }

        /// <summary>
        /// Resets the class to use DefaultEnvironmentProvider
        /// </summary>
        public static void Reset()
        {
            s_Provider = new DefaultEnvironmentProvider();
        }

        /// <summary>
        /// Sets the environment provider to the specified one
        /// </summary>
        /// <param name="provider">Environment provider to use</param>
        public static void SetProvider(IEnvironmentProvider provider)
        {
            s_Provider = provider;
        }

        /// <summary>
        /// Gets the name of the host computer running the system or
        /// application
        /// </summary>
        public static string GetHostName()
        {
            return s_Provider.GetHostName();
        }

        /// <summary>
        /// Gets the number of processors on the current machine
        /// </summary>
        public static int GetProcessorCount()
        {
            return s_Provider.GetProcessorCount();
        }

        /// <summary>
        /// Gets the number of milliseconds ellapsed since the system started
        /// </summary>
        public static int GetTickTickCount()
        {
            return s_Provider.GetTickTickCount();
        }
    }
}