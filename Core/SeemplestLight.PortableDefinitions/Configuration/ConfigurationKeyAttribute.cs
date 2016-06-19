using System;

namespace SeemplestLight.PortableCore.Configuration
{
    /// <summary>
    /// This attribute defines the key of the configuration
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationKeyAttribute : Attribute
    {
        /// <summary>
        /// Key value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes the attribute's value
        /// </summary>
        /// <param name="value">Initial value</param>
        public ConfigurationKeyAttribute(string value)
        {
            Value = value;
        }
    }
}