using System;
using System.Collections.Generic;

namespace SeemplestLight.Core.DependencyInjection
{
    /// <summary>
    /// This class is a lightweight implementation of <see cref="IServiceRegistry"/>.
    /// </summary>
    public class ServiceRegistry : IServiceRegistry
    {
        private Dictionary<Type, List<string>> _namedInstances;
        private Dictionary<InstanceKey, Func<object>> _factories;

        /// <summary>
        /// Creates a service registry with no parent registry
        /// </summary>
        public ServiceRegistry()
        {
        }

        /// <summary>
        /// Creates a service registry with the specified parent registry
        /// </summary>
        /// <param name="parent">Parent service registry</param>
        /// <remarks>
        /// If a particular service cannot be resolved in this registry,
        /// the search goes on the parent registry chain.
        /// </remarks>
        public ServiceRegistry(IServiceRegistry parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Gets the service instance with the type specified in the input parameter
        /// </summary>
        /// <param name="service">Type of the service</param>
        /// <returns>Service object, if the specified service is found; otherwise, null</returns>
        public object GetService(Type service)
        {
            var serviceObj = ObtainService(service);
            return serviceObj ?? Parent?.GetService(service);
        }

        /// <summary>
        /// Gets the named service instance with the type specified in the input parameter
        /// </summary>
        /// <param name="service">Type of the service</param>
        /// <param name="name">Service instance name</param>
        /// <returns>Service object, if the specified service is found; otherwise, null</returns>
        public object GetService(Type service, string name)
        {
            var serviceObj = ObtainService(service, name);
            return serviceObj ?? Parent?.GetService(service, name);
        }

        /// <summary>
        /// Gets the service instance with the type specified in <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Service object, if the specified service is found; otherwise, null</returns>
        public TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }

        /// <summary>
        /// Gets the named service instance with the type specified in <typeparamref name="TService"/>
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <param name="name">Service instance name</param>
        /// <returns>Service object, if the specified service is found; otherwise, null</returns>
        public TService GetService<TService>(string name)
        {
            return (TService)GetService(typeof(TService), name);
        }

        /// <summary>
        /// Resets the instance in its original states. This deletes all the
        /// registrations.
        /// </summary>
        public void Reset()
        {
            _namedInstances = new Dictionary<Type, List<string>>();
            _factories = new Dictionary<InstanceKey, Func<object>>();
        }

        /// <summary>
        /// Gets a value indicating whether a service type <typeparamref name="TInterface"/> 
        /// is already registered.
        /// </summary>
        /// <typeparam name="TInterface">The type that the method checks for.</typeparam>
        /// <returns>True if the type is registered, false otherwise.</returns>
        public bool IsRegistered<TInterface>()
        {
            return IsRegistered(typeof(TInterface), null);
        }

        /// <summary>
        /// Gets a value indicating whether a given service type is already registered.
        /// </summary>
        /// <param name="serviceType">The type that the method checks for.</param>
        /// <returns>True if the type is registered, false otherwise.</returns>
        public bool IsRegistered(Type serviceType)
        {
            return IsRegistered(serviceType, null);
        }

        /// <summary>
        /// Gets a value indicating whether a given type <typeparamref name="TInterface"/> and a give name
        /// are already registered.
        /// </summary>
        /// <typeparam name="TInterface">The type that the method checks for.</typeparam>
        /// <param name="name">The name that the method checks for.</param>
        /// <returns>True if the type and name are registered, false otherwise.</returns>
        public bool IsRegistered<TInterface>(string name)
        {
            return IsRegistered(typeof(TInterface), name);
        }

        /// <summary>
        /// Gets a value indicating whether a given service type with a given 
        /// instance name is already registered.
        /// </summary>
        /// <param name="serviceType">The type that the method checks for.</param>
        /// <param name="name">The name that the method checks for.</param>
        /// <returns>True if the type is registered, false otherwise.</returns>
        public bool IsRegistered(Type serviceType, string name)
        {
            return name == null
                ? _factories.ContainsKey(new InstanceKey(serviceType, null))
                : _namedInstances.ContainsKey(serviceType) 
                    && _namedInstances[serviceType].IndexOf(name) >= 0;
        }

        /// <summary>
        /// Registers a given instance for a given service type.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        public void Register<TInterface>(Func<TInterface> factory) where TInterface : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers a given instance for a given service type and a given name.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="name">The name for which the given instance is registered.</param>
        public void Register<TInterface>(Func<TInterface> factory, string name) where TInterface : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unregisters a service from the cache and removes all the previously
        /// created instances.
        /// </summary>
        /// <typeparam name="TInterface">The service that must be removed.</typeparam>
        public void Unregister<TInterface>() where TInterface : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the instance corresponding to the given name from the cache. 
        /// The service itself remains registered and can be used to create other instances.
        /// </summary>
        /// <typeparam name="TInterface">The type of the instance to be removed.</typeparam>
        /// <param name="key">The name corresponding to the instance that must be removed.</param>
        public void Unregister<TInterface>(string key) where TInterface : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parent of this service registry.
        /// </summary>
        /// <remarks>
        /// When a service is not found, as a fallback, a registry can check its parent registry.
        /// </remarks>
        public IServiceRegistry Parent { get; private set; }

        /// <summary>
        /// This method resolves a service object request
        /// </summary>
        /// <param name="service">Service object type</param>
        /// <param name="name">Service instance name</param>
        /// <returns>Service instance, if found; otherwise, null</returns>
        private object ObtainService(Type service, string name = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This type represents the key of a service instance
        /// </summary>
        private struct InstanceKey
        {
            /// <summary>
            /// Service type
            /// </summary>
            public Type Type { get; }

            /// <summary>
            /// Optional service name
            /// </summary>
            public string Name { get; }

            public InstanceKey(Type type, string name)
            {
                Type = type;
                Name = name;
            }

            private bool Equals(InstanceKey other)
            {
                return Type == other.Type && string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is InstanceKey && Equals((InstanceKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Name?.GetHashCode() ?? 0);
                }
            }
        }
    }
}