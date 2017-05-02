using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using SeemplestLight.Core.Portable.AbstractFiles;

namespace SeemplestLight.Core.Azure.AbstractFiles
{
    /// <summary>
    /// This class represents an Azure Storage that can retrieve files from
    /// the blob storage.
    /// </summary>
    public class AzureFileStorage: IAbstractFileStorage
    {
        /// <summary>
        /// The storage account to access the blob service
        /// </summary>
        public CloudStorageAccount StorageAccount { get; }

        /// <summary>
        /// Initializes the storage with the specified information
        /// </summary>
        /// <param name="storageInfo"></param>
        public AzureFileStorage(string storageInfo)
        {
            if (storageInfo == null)
            {
                throw new ArgumentNullException(nameof(storageInfo));
            }
            var parts = storageInfo.Split('=');
            if (parts.Length == 2 && parts[0] == "appSettingsKey")
            {
                // --- Storage account is defined in the application configuration
                storageInfo = CloudConfigurationManager.GetSetting(parts[1]);
            }
            StorageAccount = CloudStorageAccount.Parse(storageInfo);
        }

        /// <summary>
        /// Gets the containers available within the storage
        /// </summary>
        /// <returns></returns>
        public Task<List<string>> GetContainers()
        {
            return Task.Run(() =>
            {
                var blobClient = StorageAccount.CreateCloudBlobClient();
                return blobClient.ListContainers().Select(c => c.Name).ToList();
            });
        }

        /// <summary>
        /// Checks whether the container exists within the storage
        /// </summary>
        /// <param name="containerName">Name of the container to check</param>
        /// <returns>True, if the container exists; otherwise, false</returns>
        public Task<bool> ContainerExists(string containerName)
        {
            containerName = containerName.ToLower();
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            return container.ExistsAsync();
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
            containerName = containerName.ToLower();
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            if (await container.ExistsAsync())
            {
                throw new InvalidOperationException($"{containerName} container already exists, it cannot be created.");
            }
            await container.CreateAsync();
        }

        /// <summary>
        /// Removes a container from the storage, provided, it is empty
        /// </summary>
        /// <param name="containerName">Name of the container to remove</param>
        /// <param name="eraseContents"></param>
        /// <returns>True, if the container has been removed; otherwise, false</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The container is not empty, thus it cannot be removed.
        /// </exception>
        public async Task<bool> RemoveContainer(string containerName, bool eraseContents = false)
        {
            containerName = containerName.ToLower();
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                return false;
            }

            // --- Check if the container is empty
            var blob = container.ListBlobs().FirstOrDefault();
            if (!eraseContents && blob != null)
            {
                throw new InvalidOperationException($"{containerName} is not empty, it cannot be removed");
            }
            await container.DeleteAsync();
            return true;
        }
    }
}