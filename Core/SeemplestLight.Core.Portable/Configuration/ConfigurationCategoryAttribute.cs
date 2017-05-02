using System;

namespace SeemplestLight.Core.Portable.Configuration
{
    /// <summary>
    /// This attribute defines the configuration category
    /// </summary>
    /// <remarks>
    /// Add this attribute to a class that defines configuration settings 
    /// to use the specified <see cref="Name"/> for the configuration
    /// category.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationCategoryAttribute : Attribute
    {
        /// <summary>
        /// Configuration category name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes the configuration category name
        /// </summary>
        /// <param name="name">Initial name</param>
        public ConfigurationCategoryAttribute(string name)
        {
            Name = name;
        }
    }
}