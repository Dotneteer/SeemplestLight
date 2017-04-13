namespace SeemplestLight.Core.Configuration
{
    /// <summary>
    /// This interface defines the operations to read configuration values.
    /// </summary>
    public interface IConfigurationReader
    {
        /// <summary>
        /// Reads the configuration setting with the specified category and key
        /// </summary>
        /// <param name="category">Category of the configuration setting</param>
        /// <param name="key">The key of the configuration setting</param>
        /// <param name="value">Configuration value, provided, it is found</param>
        /// <returns>True, if the configuration value is found; otherwise, false</returns>
        bool GetConfigurationValue(string category, string key, out string value);
    }
}