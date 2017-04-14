using System;
using System.Collections.Generic;

namespace SeemplestLight.Core.DependencyInjection
{
    /// <summary>
    /// This exception indicates that a service instance cannot be resolved.
    /// </summary>
    public class ServiceNotFoundException : KeyNotFoundException
    {
        /// <summary>
        /// The type of the service that cannot be resolved
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Service instance name (null, if the instance is not named)
        /// </summary>
        public string InstanceName { get; }

        /// <summary>
        /// Initialiyes the exception
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="instanceName">Service instance name</param>
        public ServiceNotFoundException(Type serviceType, string instanceName) : 
            base($"The specified service ({serviceType.FullName}) cannot be resolved." 
                + (instanceName == null ? "" : " Instance name: " + instanceName))
        {
            ServiceType = serviceType;
            InstanceName = instanceName;
        }
    }
}