using System;

namespace SeemplestLight.Core.DependencyInjection
{
    /// <summary>
    /// This interface defines the behavior of a service locator
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Gets the service instance with the type specified in the input parameter
        /// </summary>
        /// <param name="service">Type of the service</param>
        /// <returns>Service object, if the specified service is found; otherwise, null</returns>
        object GetService(Type service);

        /// <summary>
        /// Gets the named service instance with the type specified in the input parameter
        /// </summary>
        /// <param name="service">Type of the service</param>
        /// <param name="name">Service instance name</param>
        /// <returns>Service object, if the specified service is found; otherwise, null</returns>
        object GetService(Type service, string name);

        /// <summary>
        /// Gets the service instance with the type specified in <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Service object, if the specified service is found; otherwise, null</returns>
        TService GetService<TService>();

        /// <summary>
        /// Gets the named service instance with the type specified in <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="name">Service instance name</param>
        /// <returns>Service object, if the specified service is found; otherwise, null</returns>
        TService GetService<TService>(string name);
    }
}