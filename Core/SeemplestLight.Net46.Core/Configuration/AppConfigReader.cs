using System;
using System.Collections.Generic;
using System.Configuration;
using SeemplestLight.PortableCore.Configuration;

namespace SeemplestLight.Net46.Core.Configuration
{
    /// <summary>
    /// This class uses the App.Config file to read configuration values
    /// </summary>
    public class AppConfigReader : IConfigurationReader
    {
        public const string MACHINE_PREFIXES = "MachinePrefixes";
        public static Dictionary<string, string> MachinePrefixes { get; private set; }
        public static string CurrentPrefix { get; private set; }

        /// <summary>
        /// Initializes the static properties of this class
        /// </summary>
        static AppConfigReader()
        {
            Reset();
        }

        /// <summary>
        /// Reads the optional machine sections from the configuration
        /// </summary>
        public static void Reset()
        {
            MachinePrefixes = null;
            CurrentPrefix = null;
            var prefixes = ConfigurationManager.AppSettings[MACHINE_PREFIXES];
            if (prefixes == null) return;
            try
            {
                MachinePrefixes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                var sections = prefixes.Split(';');
                foreach (var section in sections)
                {
                    var parts = section.Split('/');
                    if (parts.Length >= 2
                        && !string.IsNullOrWhiteSpace(parts[0])
                        && !string.IsNullOrWhiteSpace(parts[1]))
                    {
                        MachinePrefixes[parts[0]] = parts[1];
                    }
                }
                CurrentPrefix = GetPrefix(Environment.MachineName);
            }
            catch (SystemException)
            {
                // --- This exception is intentionally caught
            }
        }

        /// <summary>
        /// Ignores the optional machine sections
        /// </summary>
        public static void IgnoreMachineSections()
        {
            MachinePrefixes = null;
            CurrentPrefix = null;
        }

        /// <summary>
        /// Pretends as if the current machine were the specified one
        /// </summary>
        /// <param name="machine"></param>
        public static void PretendMachine(string machine)
        {
            CurrentPrefix = GetPrefix(machine);
        }

        /// <summary>
        /// Gets the prefix assigned to the specified machine
        /// </summary>
        /// <param name="machine"></param>
        /// <returns></returns>
        private static string GetPrefix(string machine)
        {
            string prefix = null;
            if (machine != null)
            {
                MachinePrefixes.TryGetValue(machine, out prefix);
            }
            return prefix;
        }

        /// <summary>
        /// Reads the configuration value with the specified key
        /// </summary>
        /// <param name="category">Category of the configuration item</param>
        /// <param name="key">The key of the configuration item</param>
        /// <param name="value">Configuration value, provided, it is found</param>
        /// <returns>True, if the configuration value is found; otherwise, false</returns>
        public bool GetConfigurationValue(string category, string key, out string value)
        {
            var configKey = $"{CurrentPrefix ?? ""}{category}.{key}";
            var configValue = ConfigurationManager.AppSettings[configKey];
            value = null;
            if (configValue == null)
            {
                return false;
            }
            value = configValue;
            return true;
        }
    }
}