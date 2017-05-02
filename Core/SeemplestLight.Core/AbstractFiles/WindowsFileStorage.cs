using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SeemplestLight.Core.Portable.AbstractFiles;

namespace SeemplestLight.Core.AbstractFiles
{
    /// <summary>
    /// This class represents a Windows file storage that can retrieve files
    /// from the file system
    /// </summary>
    public class WindowsFileStorage: IAbstractFileStorage
    {
        /// <summary>
        /// The root folder in the file system that represents the storage
        /// </summary>
        public string RootFolder { get; }

        public WindowsFileStorage(string rootFolder)
        {
            RootFolder = rootFolder ?? throw new ArgumentNullException(nameof(rootFolder));
        }

        /// <summary>
        /// Gets the containers available within the storage
        /// </summary>
        /// <returns></returns>
        public Task<List<string>> GetContainers()
        {
            return Task.Run(() =>
            {
                var dirInfo = new DirectoryInfo(RootFolder);
                return dirInfo.GetDirectories().Select(d => d.Name).ToList();
            });
        }

        /// <summary>
        /// Checks whether the container exists within the storage
        /// </summary>
        /// <param name="containerName">Name of the container to check</param>
        /// <returns>True, if the container exists; otherwise, false</returns>
        public Task<bool> ContainerExists(string containerName)
        {
            return Task.Run(() => Directory.Exists(Path.Combine(RootFolder, containerName)));
        }

        /// <summary>
        /// Creates a new container within the storage
        /// </summary>
        /// <param name="containerName">Name of the new container</param>
        /// <exception cref="System.InvalidOperationException">
        /// The container already exists, thus it cannot be created.
        /// </exception>
        public async Task CreateContainer(string containerName)
        {
            if (await ContainerExists(containerName))
            {
                throw new InvalidOperationException($"{containerName} container already exists, it cannot be created.");
            }
            Directory.CreateDirectory(Path.Combine(RootFolder, containerName));
        }

        /// <summary>
        /// Removes a container from the storage, provided, it is empty
        /// </summary>
        /// <param name="containerName">Name of the container to remove</param>
        /// <returns>True, if the container has been removed; otherwise, false</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The container is not empty, thus it cannot be removed.
        /// </exception>
        public Task<bool> RemoveContainer(string containerName)
        {
            return Task.Run(() =>
            {
                var containerPath = Path.Combine(RootFolder, containerName);
                if (!Directory.Exists(containerPath))
                {
                    return false;
                }
                if (Directory.GetFiles(containerPath).Length > 0 
                    || Directory.GetDirectories(containerPath).Length > 0)
                {
                    throw new InvalidOperationException($"{containerName} is not empty, it cannot be removed");
                }
                Directory.Delete(containerPath, true);
                return true;
            });
        }
    }
}