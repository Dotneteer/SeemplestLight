using System;


namespace SeemplestLight.Core.DependencyInjection
{
    /// <summary>
    /// This interface defines the responsibility of a service registry that
    /// can store and retrieve (optionally named) service instances
    /// </summary>
    public interface IServiceRegistry: IServiceLocator
    {
        /// <summary>
        /// Resets the instance in its original states. This deletes all the
        /// registrations.
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets a value indicating whether a service type <typeparamref name="TInterface"/> 
        /// is already registered.
        /// </summary>
        /// <typeparam name="TInterface">The type that the method checks for.</typeparam>
        /// <returns>True if the type is registered, false otherwise.</returns>
        bool IsRegistered<TInterface>();

        /// <summary>
        /// Gets a value indicating whether a given service type is already registered.
        /// </summary>
        /// <param name="serviceType">The type that the method checks for.</param>
        /// <returns>True if the type is registered, false otherwise.</returns>
        bool IsRegistered(Type serviceType);

        /// <summary>
        /// Gets a value indicating whether a given type <typeparamref name="TInterface"/> and a give name
        /// are already registered.
        /// </summary>
        /// <typeparam name="TInterface">The type that the method checks for.</typeparam>
        /// <param name="name">The name that the method checks for.</param>
        /// <returns>True if the type and name are registered, false otherwise.</returns>
        bool IsRegistered<TInterface>(string name);

        /// <summary>
        /// Gets a value indicating whether a given service type with a given 
        /// instance name is already registered.
        /// </summary>
        /// <param name="serviceType">The type that the method checks for.</param>
        /// <param name="name">The name that the method checks for.</param>
        /// <returns>True if the type is registered, false otherwise.</returns>
        bool IsRegistered(Type serviceType, string name);

        /// <summary>
        /// Registers a given instance for a given service type.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        void Register<TInterface>(Func<TInterface> factory)
            where TInterface : class;

        /// <summary>
        /// Registers a given instance for a given service type and a given name.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="name">The name for which the given instance is registered.</param>
        void Register<TInterface>(Func<TInterface> factory, string name)
            where TInterface : class;

        /// <summary>
        /// Unregisters a service from the cache and removes all the previously
        /// created instances.
        /// </summary>
        /// <typeparam name="TInterface">The service that must be removed.</typeparam>
        void Unregister<TInterface>()
            where TInterface : class;

        /// <summary>
        /// Removes the instance corresponding to the given name from the cache. 
        /// The service itself remains registered and can be used to create other instances.
        /// </summary>
        /// <typeparam name="TInterface">The type of the instance to be removed.</typeparam>
        /// <param name="key">The name corresponding to the instance that must be removed.</param>
        void Unregister<TInterface>(string key)
            where TInterface : class;

        /// <summary>
        /// Parent of this service registry.
        /// </summary>
        /// <remarks>
        /// When a service is not found, as a fallback, a registry can check its parent registry.
        /// </remarks>
        IServiceRegistry Parent { get; }
    }
}