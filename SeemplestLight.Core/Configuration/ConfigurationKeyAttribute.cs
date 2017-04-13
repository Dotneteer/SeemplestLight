using System;

namespace SeemplestLight.Core.Configuration
{
    /// <summary>
    /// This attribute defines the key of a configuration setting
    /// </summary>
    /// <remarks>
    /// Add this attribute to a configuration property to set the
    /// name its name in the configuration store.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationKeyAttribute : Attribute
    {
        /// <summary>
        /// Configuration key name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes the configuration key name
        /// </summary>
        /// <param name="name">Initial name</param>
        public ConfigurationKeyAttribute(string name)
        {
            Name = name;
        }
    }
}