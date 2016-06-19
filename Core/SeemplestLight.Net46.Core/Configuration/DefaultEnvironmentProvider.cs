using System;
using SeemplestLight.PortableCore.Configuration;

namespace SeemplestLight.Net46.Core.Configuration
{
    /// <summary>
    /// This class defines an environment provider that apps can use in production mode
    /// </summary>
    public class DefaultEnvironmentProvider: IEnvironmentProvider
    {
        /// <summary>
        /// Gets the name of the host computer running the system or
        /// application
        /// </summary>
        public string GetHostName()
        {
            return Environment.MachineName;
        }

        /// <summary>
        /// Gets the number of processors on the current machine
        /// </summary>
        public int GetProcessorCount()
        {
            return Environment.ProcessorCount;
        }

        /// <summary>
        /// Gets the number of milliseconds ellapsed since the system started
        /// </summary>
        public int GetTickTickCount()
        {
            return Environment.TickCount;
        }
    }
}