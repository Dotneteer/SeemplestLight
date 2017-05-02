using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeemplestLight.Core.Portable.AbstractFiles
{
    /// <summary>
    /// This interface describes an abstract file storage that can contain
    /// files.
    /// </summary>
    /// <remarks>All operations are async</remarks>
    public interface IAbstractFileStorage
    {
        /// <summary>
        /// Gets the containers available within the storage
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetContainers();

        /// <summary>
        /// Checks whether the container exists within the storage
        /// </summary>
        /// <param name="containerName">Name of the container to check</param>
        /// <returns>True, if the container exists; otherwise, false</returns>
        Task<bool> ContainerExists(string containerName);

        /// <summary>
        /// Creates a new container within the storage
        /// </summary>
        /// <param name="containerName">Name of the new container</param>
        /// <exception cref="InvalidOperationException">
        /// The container already exists, thus it cannot be created.
        /// </exception>
        Task CreateContainer(string containerName);

        /// <summary>
        /// Removes a container from the storage, provided, it is empty
        /// </summary>
        /// <param name="containerName">Name of the container to remove</param>
        /// <param name="eraseContents">True indicates that the container should be emptied</param>
        /// <returns>True, if the container has been removed; otherwise, false</returns>
        /// <exception cref="InvalidOperationException">
        /// The container is not empty, thus it cannot be removed.
        /// </exception>
        Task<bool> RemoveContainer(string containerName, bool eraseContents = false);
    }
}