using System;

namespace SeemplestLight.Core.Portable.DependencyInjection
{
    /// <summary>
    /// This class provides a singleston (static) object to access 
    /// a default service registry.
    /// </summary>
    public static class ServiceManager
    {
        /// <summary>
        /// The registry used by ServiceManager
        /// </summary>
        public static IServiceRegistry Registry { get; private set; }

        /// <summary>
        /// Initializes the static members of the class
        /// </summary>
        static ServiceManager()
        {
            Reset();
        }

        /// <summary>
        /// Resets ServiceManager to its initial state just like right
        /// after the framework invokes its static constructor
        /// </summary>
        public static void Reset()
        {
            Registry = new ServiceRegistry();
        }

        /// <summary>
        /// Sets the registry instance ServiceManager works with
        /// </summary>
        /// <param name="registry"></param>
        public static void SetRegistry(IServiceRegistry registry)
        {
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        /// <summary>
        /// Gets the service instance with the type specified in the input parameter
        /// </summary>
        /// <param name="serviceType">Type of the service</param>
        /// <returns>Service object, if the specified service is found</returns>
        /// <exception cref="ServiceNotFoundException">The service instance cannot be found</exception>
        public static object GetService(Type serviceType)
        {
            return Registry.GetService(serviceType);
        }

        /// <summary>
        /// Gets the named service instance with the type specified in the input parameter
        /// </summary>
        /// <param name="serviceType">Type of the service</param>
        /// <param name="name">Service instance name</param>
        /// <returns>Service object, if the specified service is found</returns>
        /// <exception cref="ServiceNotFoundException">The service instance cannot be found</exception>
        public static object GetService(Type serviceType, string name)
        {
            return Registry.GetService(serviceType, name);
        }

        /// <summary>
        /// Gets the service instance with the type specified in <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Service object, if the specified service is found</returns>
        /// <exception cref="ServiceNotFoundException">The service instance cannot be found</exception>
        public static TService GetService<TService>()
        {
            return Registry.GetService<TService>();
        }

        /// <summary>
        /// Gets the named service instance with the type specified in <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="name">Service instance name</param>
        /// <returns>Service object, if the specified service is found</returns>
        /// <exception cref="ServiceNotFoundException">The service instance cannot be found</exception>
        public static TService GetService<TService>(string name)
        {
            return Registry.GetService<TService>(name);
        }

        /// <summary>
        /// Resets the instance in its original states. This deletes all the
        /// registrations.
        /// </summary>
        public static void ResetRegistry()
        {
            Registry.Reset();
        }

        /// <summary>
        /// Gets a value indicating whether a service type <typeparamref name="TInterface"/> 
        /// is already registered.
        /// </summary>
        /// <typeparam name="TInterface">The type that the method checks for.</typeparam>
        /// <returns>True if the type is registered; otherwise, false.</returns>
        public static bool IsRegistered<TInterface>()
        {
            return Registry.IsRegistered<TInterface>();
        }

        /// <summary>
        /// Gets a value indicating whether a given service type is already registered.
        /// </summary>
        /// <param name="serviceType">The type that the method checks for.</param>
        /// <returns>True if the type is registered; otherwise, false.</returns>
        public static bool IsRegistered(Type serviceType)
        {
            return Registry.IsRegistered(serviceType);
        }

        /// <summary>
        /// Gets a value indicating whether a given type <typeparamref name="TInterface"/> and a give name
        /// are already registered.
        /// </summary>
        /// <typeparam name="TInterface">The type that the method checks for.</typeparam>
        /// <param name="name">The name that the method checks for.</param>
        /// <returns>True if the type is registered; otherwise, false.</returns>
        public static bool IsRegistered<TInterface>(string name)
        {
            return Registry.IsRegistered<TInterface>(name);
        }

        /// <summary>
        /// Gets a value indicating whether a given service type with a given 
        /// instance name is already registered.
        /// </summary>
        /// <param name="serviceType">The type that the method checks for.</param>
        /// <param name="name">The name that the method checks for.</param>
        /// <returns>True if the type is registered; otherwise, false.</returns>
        public static bool IsRegistered(Type serviceType, string name)
        {
            return Registry.IsRegistered(serviceType, name);
        }

        /// <summary>
        /// Registers a given instance for a given service type.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">
        /// The factory method able to create the instance that
        /// must be returned when the given type is resolved.
        /// </param>
        public static void Register<TInterface>(Func<TInterface> factory)
            where TInterface : class
        {
            Registry.Register(factory);
        }

        /// <summary>
        /// Registers a given instance for a given service type and a given name.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">
        /// The factory method able to create the instance that
        /// must be returned when the given type is resolved.
        /// </param>
        /// <param name="name">The name for which the given instance is registered.</param>
        public static void Register<TInterface>(Func<TInterface> factory, string name)
            where TInterface : class
        {
            Registry.Register(factory, name);
        }

        /// <summary>
        /// Registers a given service type with a service object type.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <typeparam name="TService">
        /// The type of object to create whenever a service instance is requested
        /// </typeparam>
        public static void Register<TInterface, TService>()
            where TInterface : class
            where TService : class, TInterface, new()
        {
            Registry.Register(() => new TService());
        }

        /// <summary>
        /// Registers a given instance for given service type with a service object type.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <typeparam name="TService">
        /// The type of object to create whenever a service instance is requested
        /// </typeparam>
        /// <param name="name">The name for which the given instance is registered.</param>
        public static void Register<TInterface, TService>(string name)
            where TInterface : class
            where TService : class, TInterface, new()
        {
            Registry.Register(() => new TService(), name);
        }

        /// <summary>
        /// Registers a given service type with the specified service instance.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        public static void RegisterInstance<TInterface>(TInterface instance)
            where TInterface : class
        {
            Registry.Register(() => instance);
        }

        /// <summary>
        /// Unregisters a service from the cache and removes all the previously
        /// created instances.
        /// </summary>
        /// <typeparam name="TInterface">The service that must be removed.</typeparam>
        public static void Unregister<TInterface>()
            where TInterface : class
        {
            Registry.Unregister<TInterface>();
        }

        /// <summary>
        /// Removes the instance corresponding to the given name from the cache. 
        /// The service itself remains registered and can be used to create other instances.
        /// </summary>
        /// <typeparam name="TInterface">The type of the instance to be removed.</typeparam>
        /// <param name="name">The name corresponding to the instance that must be removed.</param>
        public static void Unregister<TInterface>(string name)
            where TInterface : class
        {
            Registry.Unregister<TInterface>(name);
        }
    }
}