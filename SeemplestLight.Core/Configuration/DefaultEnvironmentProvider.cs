using System;

namespace SeemplestLight.Core.Configuration
{
    /// <summary>
    /// This class defines an environment provider that apps can use in production mode
    /// </summary>
    public class DefaultEnvironmentProvider : IEnvironmentProvider
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
        /// Gets the name of the user currently logged into the machine
        /// </summary>
        public string GetUserName()
        {
            return Environment.UserName;
        }

        /// <summary>
        /// Gets the domain name of the user currently logged into the machine
        /// </summary>
        public string GetUserDomainName()
        {
            return Environment.UserDomainName;
        }

        /// <summary>
        /// Gets the number of milliseconds ellapsed since the system started
        /// </summary>
        public int GetTickCount()
        {
            return Environment.TickCount;
        }
    }
}