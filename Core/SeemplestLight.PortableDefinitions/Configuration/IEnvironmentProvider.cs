namespace SeemplestLight.PortableCore.Configuration
{
    /// <summary>
    /// This interface defines operations to get information about the environment
    /// </summary>
    public interface IEnvironmentProvider
    {
        /// <summary>
        /// Gets the name of the host computer running the system or
        /// application
        /// </summary>
        string GetHostName();

        /// <summary>
        /// Gets the number of processors on the current machine
        /// </summary>
        int GetProcessorCount();

        /// <summary>
        /// Gets the number of milliseconds ellapsed since the system started
        /// </summary>
        int GetTickTickCount();
    }
}