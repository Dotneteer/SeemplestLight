﻿// Copyright © GalaSoft Laurent Bugnion 2011-2016
// See license.txt in this project or http://www.galasoft.ch/license_MIT.txt
// 
// Refactored by Istvan Novak

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SeemplestLight.PortableCore.IoC
{
    /// <summary>
    /// A simplified IoC container to locate and register service instances.
    /// </summary>
    /// <remarks>I was inspired by Laurent Bugnion's SimleIoc container.</remarks>
    public class ServiceRegistry : IServiceContainer
    {
        private readonly Dictionary<Type, ConstructorInfo> _constructorInfo = 
            new Dictionary<Type, ConstructorInfo>();

        private readonly string _defaultKey = Guid.NewGuid().ToString();

        private readonly object[] _emptyArguments = new object[0];

        private readonly Dictionary<Type, Dictionary<string, Delegate>> _factories = 
            new Dictionary<Type, Dictionary<string, Delegate>>();

        private readonly Dictionary<Type, Dictionary<string, object>> _instancesRegistry = 
            new Dictionary<Type, Dictionary<string, object>>();

        private readonly Dictionary<Type, Type> _interfaceToClassMap = 
            new Dictionary<Type, Type>();

        private readonly object _syncLock = new object();

        private static ServiceRegistry s_Default;

        /// <summary>
        /// This class' default instance.
        /// </summary>
        public static ServiceRegistry Default => s_Default ?? (s_Default = new ServiceRegistry());

        /// <summary>
        /// Checks whether at least one instance of a given class is already created in the container.
        /// </summary>
        /// <typeparam name="TInterface">The class that is queried.</typeparam>
        /// <returns>True if at least on instance of the class is already created, false otherwise.</returns>
        public bool ContainsCreated<TInterface>()
        {
            return ContainsCreated<TInterface>(null);
        }

        /// <summary>
        /// Checks whether the instance with the given key is already created for a given class
        /// in the container.
        /// </summary>
        /// <typeparam name="TInterface">The class that is queried.</typeparam>
        /// <param name="key">The key that is queried.</param>
        /// <returns>True if the instance with the given key is already registered for the given class,
        /// false otherwise.</returns>
        public bool ContainsCreated<TInterface>(string key)
        {
            var classType = typeof(TInterface);
            if (!_instancesRegistry.ContainsKey(classType))
            {
                return false;
            }
            if (string.IsNullOrEmpty(key))
            {
                return _instancesRegistry[classType].Count > 0;
            }
            return _instancesRegistry[classType].ContainsKey(key);
        }

        /// <summary>
        /// Gets a value indicating whether a given type T is already registered.
        /// </summary>
        /// <typeparam name="T">The type that the method checks for.</typeparam>
        /// <returns>True if the type is registered, false otherwise.</returns>
        public bool IsRegistered<T>()
        {
            var classType = typeof(T);
            return _interfaceToClassMap.ContainsKey(classType);
        }

        /// <summary>
        /// Gets a value indicating whether a given type T and a give key
        /// are already registered.
        /// </summary>
        /// <typeparam name="T">The type that the method checks for.</typeparam>
        /// <param name="key">The key that the method checks for.</param>
        /// <returns>True if the type and key are registered, false otherwise.</returns>
        public bool IsRegistered<T>(string key)
        {
            var classType = typeof(T);

            lock (_syncLock)
            {
                if (!_interfaceToClassMap.ContainsKey(classType)
                    || !_factories.ContainsKey(classType))
                {
                    return false;
                }
            }

            lock (_syncLock)
            {
                return _factories[classType].ContainsKey(key);
            }
        }

        /// <summary>
        /// Registers a given type.
        /// </summary>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        public void Register<TClass>()
            where TClass : class
        {
            Register<TClass>(false);
        }

        /// <summary>
        /// Registers a given type with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        public void Register<TClass>(bool createInstanceImmediately)
            where TClass : class
        {
            var classType = typeof(TClass);
            if (classType.GetTypeInfo().IsInterface)
            {
                throw new ArgumentException("An interface cannot be registered alone.");
            }

            lock (_syncLock)
            {
                if (_factories.ContainsKey(classType)
                    && _factories[classType].ContainsKey(_defaultKey))
                {
                    if (!_constructorInfo.ContainsKey(classType))
                    {
                        // Throw only if constructorinfos have not been
                        // registered, which means there is a default factory
                        // for this class.
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Class {0} is already registered.",
                                classType));
                    }

                    return;
                }

                if (!_interfaceToClassMap.ContainsKey(classType))
                {
                    _interfaceToClassMap.Add(classType, null);
                }

                _constructorInfo.Add(classType, GetConstructorInfo(classType));
                Func<TClass> factory = MakeInstance<TClass>;
                DoRegister(classType, factory, _defaultKey);

                if (createInstanceImmediately)
                {
                    GetInstance<TClass>();
                }
            }
        }


        /// <summary>
        /// Registers a given type for a given interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface for which instances will be resolved.</typeparam>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        public void Register<TInterface, TClass>()
            where TClass : class
            where TInterface : class
        {
            Register<TInterface, TClass>(false);
        }

        /// <summary>
        /// Registers a given type for a given interface with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TInterface">The interface for which instances will be resolved.</typeparam>
        /// <typeparam name="TClass">The type that must be used to create instances.</typeparam>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        public void Register<TInterface, TClass>(bool createInstanceImmediately)
            where TClass : class
            where TInterface : class
        {
            var interfaceType = typeof(TInterface);
            var classType = typeof(TClass);
            lock (_syncLock)
            {
                if (_interfaceToClassMap.ContainsKey(interfaceType))
                {
                    if (_interfaceToClassMap[interfaceType] != classType)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "There is already a class registered for {0}.",
                                interfaceType.FullName));
                    }
                }
                else
                {
                    _interfaceToClassMap.Add(interfaceType, classType);
                    _constructorInfo.Add(classType, GetConstructorInfo(classType));
                }

                Func<TInterface> factory = MakeInstance<TInterface>;
                DoRegister(interfaceType, factory, _defaultKey);

                if (createInstanceImmediately)
                {
                    GetInstance<TInterface>();
                }
            }
        }

        /// <summary>
        /// Registers a given instance for a given type.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        public void Register<TInterface>(Func<TInterface> factory)
            where TInterface : class
        {
            Register(factory, false);
        }

        /// <summary>
        /// Registers a given instance for a given type with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        public void Register<TInterface>(Func<TInterface> factory, bool createInstanceImmediately)
            where TInterface : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var classType = typeof(TInterface);

            lock (_syncLock)
            {
                if (_factories.ContainsKey(classType)
                    && _factories[classType].ContainsKey(_defaultKey))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "There is already a factory registered for {0}.",
                            classType.FullName));
                }

                if (!_interfaceToClassMap.ContainsKey(classType))
                {
                    _interfaceToClassMap.Add(classType, null);
                }

                DoRegister(classType, factory, _defaultKey);
                if (createInstanceImmediately)
                {
                    GetInstance<TInterface>();
                }
            }
        }

        /// <summary>
        /// Registers a given instance for a given type and a given key.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="key">The key for which the given instance is registered.</param>
        public void Register<TInterface>(Func<TInterface> factory, string key)
            where TInterface : class
        {
            Register(factory, key, false);
        }

        /// <summary>
        /// Registers a given instance for a given type and a given key with the possibility for immediate
        /// creation of the instance.
        /// </summary>
        /// <typeparam name="TInterface">The type that is being registered.</typeparam>
        /// <param name="factory">The factory method able to create the instance that
        /// must be returned when the given type is resolved.</param>
        /// <param name="key">The key for which the given instance is registered.</param>
        /// <param name="createInstanceImmediately">If true, forces the creation of the default
        /// instance of the provided class.</param>
        public void Register<TInterface>(
            Func<TInterface> factory,
            string key,
            bool createInstanceImmediately)
            where TInterface : class
        {
            var classType = typeof(TInterface);
            lock (_syncLock)
            {
                if (_factories.ContainsKey(classType)
                    && _factories[classType].ContainsKey(key))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "There is already a factory registered for {0} with key {1}.",
                            classType.FullName,
                            key));
                }

                if (!_interfaceToClassMap.ContainsKey(classType))
                {
                    _interfaceToClassMap.Add(classType, null);
                }

                DoRegister(classType, factory, key);

                if (createInstanceImmediately)
                {
                    GetInstance<TInterface>(key);
                }
            }
        }

        /// <summary>
        /// Resets the instance in its original states. This deletes all the
        /// registrations.
        /// </summary>
        public void Reset()
        {
            lock (_syncLock)
            {
                _interfaceToClassMap.Clear();
                _instancesRegistry.Clear();
                _constructorInfo.Clear();
                _factories.Clear();
            }
        }

        /// <summary>
        /// Unregisters a class from the cache and removes all the previously
        /// created instances.
        /// </summary>
        /// <typeparam name="TInterface">The class that must be removed.</typeparam>
        public void Unregister<TInterface>()
            where TInterface : class
        {
            var serviceType = typeof(TInterface);

            lock (_syncLock)
            {
                Type resolveTo;

                if (_interfaceToClassMap.ContainsKey(serviceType))
                {
                    resolveTo = _interfaceToClassMap[serviceType] ?? serviceType;
                }
                else
                {
                    resolveTo = serviceType;
                }

                if (_instancesRegistry.ContainsKey(serviceType))
                {
                    _instancesRegistry.Remove(serviceType);
                }

                if (_interfaceToClassMap.ContainsKey(serviceType))
                {
                    _interfaceToClassMap.Remove(serviceType);
                }

                if (_factories.ContainsKey(serviceType))
                {
                    _factories.Remove(serviceType);
                }

                if (_constructorInfo.ContainsKey(resolveTo))
                {
                    _constructorInfo.Remove(resolveTo);
                }
            }
        }

        /// <summary>
        /// Removes the given instance from the cache. The class itself remains
        /// registered and can be used to create other instances.
        /// </summary>
        /// <typeparam name="TInterface">The type of the instance to be removed.</typeparam>
        /// <param name="instance">The instance that must be removed.</param>
        public void Unregister<TInterface>(TInterface instance)
            where TInterface : class
        {
            var classType = typeof(TInterface);
            lock (_syncLock)
            {
                if (!_instancesRegistry.ContainsKey(classType))
                {
                    return;
                }

                var list = _instancesRegistry[classType];

                var pairs = list.Where(pair => pair.Value == instance).ToList();
                for (var index = 0; index < pairs.Count(); index++)
                {
                    var key = pairs[index].Key;

                    list.Remove(key);
                }
            }
        }

        /// <summary>
        /// Removes the instance corresponding to the given key from the cache. The class itself remains
        /// registered and can be used to create other instances.
        /// </summary>
        /// <typeparam name="TInterface">The type of the instance to be removed.</typeparam>
        /// <param name="key">The key corresponding to the instance that must be removed.</param>
        public void Unregister<TInterface>(string key)
            where TInterface : class
        {
            var classType = typeof(TInterface);

            lock (_syncLock)
            {
                if (_instancesRegistry.ContainsKey(classType))
                {
                    var list = _instancesRegistry[classType];

                    var pairs = list.Where(pair => pair.Key == key).ToList();
                    for (var index = 0; index < pairs.Count(); index++)
                    {
                        list.Remove(pairs[index].Key);
                    }
                }

                if (!_factories.ContainsKey(classType))
                {
                    return;
                }

                if (_factories[classType].ContainsKey(key))
                {
                    _factories[classType].Remove(key);
                }
            }
        }

        private object DoGetService(Type serviceType, string key, bool cache = true)
        {
            lock (_syncLock)
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = _defaultKey;
                }

                Dictionary<string, object> instances = null;

                if (!_instancesRegistry.ContainsKey(serviceType))
                {
                    if (!_interfaceToClassMap.ContainsKey(serviceType))
                    {
                        throw new ActivationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Type not found in cache: {0}.",
                                serviceType.FullName));
                    }

                    if (cache)
                    {
                        instances = new Dictionary<string, object>();
                        _instancesRegistry.Add(serviceType, instances);
                    }
                }
                else
                {
                    instances = _instancesRegistry[serviceType];
                }

                if (instances != null
                    && instances.ContainsKey(key))
                {
                    return instances[key];
                }

                object instance = null;

                if (_factories.ContainsKey(serviceType))
                {
                    if (_factories[serviceType].ContainsKey(key))
                    {
                        instance = _factories[serviceType][key].DynamicInvoke(null);
                    }
                    else
                    {
                        if (_factories[serviceType].ContainsKey(_defaultKey))
                        {
                            instance = _factories[serviceType][_defaultKey].DynamicInvoke(null);
                        }
                        else
                        {
                            throw new ActivationException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Type not found in cache without a key: {0}",
                                    serviceType.FullName));
                        }
                    }
                }

                if (cache)
                {
                    instances?.Add(key, instance);
                }

                return instance;
            }
        }

        private void DoRegister<TClass>(Type classType, Func<TClass> factory, string key)
        {
            if (_factories.ContainsKey(classType))
            {
                if (_factories[classType].ContainsKey(key))
                {
                    // The class is already registered, ignore and continue.
                    return;
                }

                _factories[classType].Add(key, factory);
            }
            else
            {
                var list = new Dictionary<string, Delegate>
                {
                    {
                        key,
                        factory
                    }
                };

                _factories.Add(classType, list);
            }
        }

        private ConstructorInfo GetConstructorInfo(Type serviceType)
        {
            Type resolveTo;

            if (_interfaceToClassMap.ContainsKey(serviceType))
            {
                resolveTo = _interfaceToClassMap[serviceType] ?? serviceType;
            }
            else
            {
                resolveTo = serviceType;
            }

            var constructorInfos = resolveTo.GetTypeInfo().DeclaredConstructors.Where(c => c.IsPublic).ToArray();

            if (constructorInfos.Length > 1)
            {
                if (constructorInfos.Length > 2)
                {
                    return GetPreferredConstructorInfo(constructorInfos, resolveTo);
                }

                if (constructorInfos.FirstOrDefault(i => i.Name == ".cctor") == null)
                {
                    return GetPreferredConstructorInfo(constructorInfos, resolveTo);
                }

                var first = constructorInfos.FirstOrDefault(i => i.Name != ".cctor");

                if (first == null
                    || !first.IsPublic)
                {
                    throw new ActivationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Cannot register: No public constructor found in {0}.",
                            resolveTo.Name));
                }

                return first;
            }

            if (constructorInfos.Length == 0
                || (constructorInfos.Length == 1
                    && !constructorInfos[0].IsPublic))
            {
                throw new ActivationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot register: No public constructor found in {0}.",
                        resolveTo.Name));
            }

            return constructorInfos[0];
        }

        private static ConstructorInfo GetPreferredConstructorInfo(IEnumerable<ConstructorInfo> constructorInfos, Type resolveTo)
        {
            var preferredConstructorInfo
                = (from t in constructorInfos
                    let attribute = t.GetCustomAttribute(typeof(PreferredConstructorAttribute))
                   where attribute != null
                   select t).FirstOrDefault();

            if (preferredConstructorInfo == null)
            {
                throw new ActivationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot register: Multiple constructors found in {0} but none marked with PreferredConstructor.",
                        resolveTo.Name));
            }

            return preferredConstructorInfo;
        }

        private TClass MakeInstance<TClass>()
        {
            var serviceType = typeof(TClass);

            var constructor = _constructorInfo.ContainsKey(serviceType)
                                  ? _constructorInfo[serviceType]
                                  : GetConstructorInfo(serviceType);

            var parameterInfos = constructor.GetParameters();

            if (parameterInfos.Length == 0)
            {
                return (TClass)constructor.Invoke(_emptyArguments);
            }

            var parameters = new object[parameterInfos.Length];

            foreach (var parameterInfo in parameterInfos)
            {
                parameters[parameterInfo.Position] = GetService(parameterInfo.ParameterType);
            }

            return (TClass)constructor.Invoke(parameters);
        }

        /// <summary>
        /// Provides a way to get all the created instances of a given type available in the
        /// cache. Registering a class or a factory does not automatically
        /// create the corresponding instance! To create an instance, either register
        /// the class or the factory with createInstanceImmediately set to true,
        /// or call the GetInstance method before calling GetAllCreatedInstances.
        /// Alternatively, use the GetAllInstances method, which auto-creates default
        /// instances for all registered classes.
        /// </summary>
        /// <param name="serviceType">The class of which all instances
        /// must be returned.</param>
        /// <returns>All the already created instances of the given type.</returns>
        public IEnumerable<object> GetAllCreatedInstances(Type serviceType)
        {
            if (_instancesRegistry.ContainsKey(serviceType))
            {
                return _instancesRegistry[serviceType].Values;
            }

            return new List<object>();
        }

        /// <summary>
        /// Provides a way to get all the created instances of a given type available in the
        /// cache. Registering a class or a factory does not automatically
        /// create the corresponding instance! To create an instance, either register
        /// the class or the factory with createInstanceImmediately set to true,
        /// or call the GetInstance method before calling GetAllCreatedInstances.
        /// Alternatively, use the GetAllInstances method, which auto-creates default
        /// instances for all registered classes.
        /// </summary>
        /// <typeparam name="TService">The class of which all instances
        /// must be returned.</typeparam>
        /// <returns>All the already created instances of the given type.</returns>
        public IEnumerable<TService> GetAllCreatedInstances<TService>()
        {
            var serviceType = typeof(TService);
            return GetAllCreatedInstances(serviceType)
                .Select(instance => (TService)instance);
        }

        #region Implementation of IServiceProvider

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        /// <returns>
        /// A service object of type <paramref name="serviceType" />.
        /// </returns>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        public object GetService(Type serviceType)
        {
            return DoGetService(serviceType, _defaultKey);
        }

        #endregion

        #region Implementation of IServiceLocator

        /// <summary>
        /// Provides a way to get all the created instances of a given type available in the
        /// cache. Calling this method auto-creates default
        /// instances for all registered classes.
        /// </summary>
        /// <param name="serviceType">The class of which all instances
        /// must be returned.</param>
        /// <returns>All the instances of the given type.</returns>
        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            lock (_factories)
            {
                if (_factories.ContainsKey(serviceType))
                {
                    foreach (var factory in _factories[serviceType])
                    {
                        GetInstance(serviceType, factory.Key);
                    }
                }
            }

            if (_instancesRegistry.ContainsKey(serviceType))
            {
                return _instancesRegistry[serviceType].Values;
            }


            return new List<object>();
        }

        /// <summary>
        /// Provides a way to get all the created instances of a given type available in the
        /// cache. Calling this method auto-creates default
        /// instances for all registered classes.
        /// </summary>
        /// <typeparam name="TService">The class of which all instances
        /// must be returned.</typeparam>
        /// <returns>All the instances of the given type.</returns>
        public IEnumerable<TService> GetAllInstances<TService>()
        {
            var serviceType = typeof(TService);
            return GetAllInstances(serviceType)
                .Select(instance => (TService)instance);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. If no instance had been instantiated 
        /// before, a new instance will be created. If an instance had already
        /// been created, that same instance will be returned.
        /// </summary>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        /// <param name="serviceType">The class of which an instance
        /// must be returned.</param>
        /// <returns>An instance of the given type.</returns>
        public object GetInstance(Type serviceType)
        {
            return DoGetService(serviceType, _defaultKey);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. This method
        /// always returns a new instance and doesn't cache it in the IOC container.
        /// </summary>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        /// <param name="serviceType">The class of which an instance
        /// must be returned.</param>
        /// <returns>An instance of the given type.</returns>
        public object GetInstanceWithoutCaching(Type serviceType)
        {
            return DoGetService(serviceType, _defaultKey, false);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type corresponding
        /// to a given key. If no instance had been instantiated with this
        /// key before, a new instance will be created. If an instance had already
        /// been created with the same key, that same instance will be returned.
        /// </summary>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        /// <param name="serviceType">The class of which an instance must be returned.</param>
        /// <param name="key">The key uniquely identifying this instance.</param>
        /// <returns>An instance corresponding to the given type and key.</returns>
        public object GetInstance(Type serviceType, string key)
        {
            return DoGetService(serviceType, key);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. This method
        /// always returns a new instance and doesn't cache it in the IOC container.
        /// </summary>
        /// <exception cref="ActivationException">If the type serviceType has not
        /// been registered before calling this method.</exception>
        /// <param name="serviceType">The class of which an instance must be returned.</param>
        /// <param name="key">The key uniquely identifying this instance.</param>
        /// <returns>An instance corresponding to the given type and key.</returns>
        public object GetInstanceWithoutCaching(Type serviceType, string key)
        {
            return DoGetService(serviceType, key, false);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. If no instance had been instantiated 
        /// before, a new instance will be created. If an instance had already
        /// been created, that same instance will be returned.
        /// </summary>
        /// <exception cref="ActivationException">If the type TService has not
        /// been registered before calling this method.</exception>
        /// <typeparam name="TService">The class of which an instance
        /// must be returned.</typeparam>
        /// <returns>An instance of the given type.</returns>
        public TService GetInstance<TService>()
        {
            return (TService)DoGetService(typeof(TService), _defaultKey);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. This method
        /// always returns a new instance and doesn't cache it in the IOC container.
        /// </summary>
        /// <exception cref="ActivationException">If the type TService has not
        /// been registered before calling this method.</exception>
        /// <typeparam name="TService">The class of which an instance
        /// must be returned.</typeparam>
        /// <returns>An instance of the given type.</returns>
        public TService GetInstanceWithoutCaching<TService>()
        {
            return (TService)DoGetService(typeof(TService), _defaultKey, false);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type corresponding
        /// to a given key. If no instance had been instantiated with this
        /// key before, a new instance will be created. If an instance had already
        /// been created with the same key, that same instance will be returned.
        /// </summary>
        /// <exception cref="ActivationException">If the type TService has not
        /// been registered before calling this method.</exception>
        /// <typeparam name="TService">The class of which an instance must be returned.</typeparam>
        /// <param name="key">The key uniquely identifying this instance.</param>
        /// <returns>An instance corresponding to the given type and key.</returns>
        public TService GetInstance<TService>(string key)
        {
            return (TService)DoGetService(typeof(TService), key);
        }

        /// <summary>
        /// Provides a way to get an instance of a given type. This method
        /// always returns a new instance and doesn't cache it in the IOC container.
        /// </summary>
        /// <exception cref="ActivationException">If the type TService has not
        /// been registered before calling this method.</exception>
        /// <typeparam name="TService">The class of which an instance must be returned.</typeparam>
        /// <param name="key">The key uniquely identifying this instance.</param>
        /// <returns>An instance corresponding to the given type and key.</returns>
        public TService GetInstanceWithoutCaching<TService>(string key)
        {
            return (TService)DoGetService(typeof(TService), key, false);
        }

        #endregion
    }
}