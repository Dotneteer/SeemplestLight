using System;

namespace SeemplestLight.PortableCore.Configuration
{
    /// <summary>
    /// This attribute defines the configuration category
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationCategoryAttribute : Attribute
    {
        /// <summary>
        /// Category value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes the attribute's value
        /// </summary>
        /// <param name="value">Initial value</param>
        public ConfigurationCategoryAttribute(string value)
        {
            Value = value;
        }
    }
}