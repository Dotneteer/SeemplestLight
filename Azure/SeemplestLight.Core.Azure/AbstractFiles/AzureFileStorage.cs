using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
        public Task<List<string>> GetContainersAsync()
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
        public Task<bool> ContainerExistsAsync(string containerName)
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
        public async Task CreateContainerAsync(string containerName)
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
        /// Creates the specified container in the storage, provided it does not exist yet
        /// </summary>
        /// <param name="containerName">Name of the new container</param>
        public async Task EnsureContainerAsync(string containerName)
        {
            containerName = containerName.ToLower();
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
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
        public async Task<bool> RemoveContainerAsync(string containerName, bool eraseContents = false)
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

        /// <summary>
        /// Checks whether the specified file exists in the storage.
        /// </summary>
        /// <param name="file">File to check</param>
        /// <returns>True, if the file exists; otherwise, false.</returns>
        public async Task<bool> ExistsAsync(AbstractFileDescriptor file)
        {
            var containerName = (file.RootContainer ?? "").ToLower();
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                return false;
            }

            var blob = await GetAppendBlobReference(file);
            return blob != null && await blob.ExistsAsync();
        }

        /// <summary>
        /// Removes the specified abstract file
        /// </summary>
        /// <param name="file">File to remove</param>
        /// <returns>True, if the file existed before the remove operation</returns>
        public async Task<bool> DeleteAsync(AbstractFileDescriptor file)
        {
            var containerName = (file.RootContainer ?? "").ToLower();
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                return false;
            }

            var blob = await GetAppendBlobReference(file);
            if (blob == null)
            {
                return false;
            }
            return await blob.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Opens a text file for read. Returns the object to work with the file.
        /// </summary>
        /// <param name="file">Abstract file descriptor</param>
        /// <param name="encoding">Optional file encoding</param>
        /// <returns>
        /// The object that provides operations to work with the text file.
        /// </returns>
        public async Task<IAbstractTextFile> OpenTextAsync(AbstractFileDescriptor file, Encoding encoding = null)
        {
            var blob = await GetAppendBlobReference(file);
            if (blob == null || !await blob.ExistsAsync())
            {
                throw new FileNotFoundException(GetBlobNameFromAbstractFile(file));
            }
            var memoryStream = new MemoryStream();
            blob.DownloadToStream(memoryStream);
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(memoryStream, encoding);
            return new AzureTextFile(reader);
        }

        /// <summary>
        /// Creates an abstract text file. Returns the object to work with the file.
        /// </summary>
        /// <param name="file">Abstract file descriptor</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="encoding">Optional file encoding</param>
        /// <param name="flushSize">
        ///     The flush size to set in Kbytes. If zero or less, the default storage provider flush
        ///     size is used.
        /// </param>
        /// <returns>
        /// The object that provides operations to work with the text file.
        /// </returns>
        public async Task<IAbstractTextFile> CreateTextAsync(AbstractFileDescriptor file, IFormatProvider formatProvider = null,
            Encoding encoding = null, int flushSize = 0)
        {
            await EnsureContainerAsync(file.RootContainer);
            var blob = await GetAppendBlobReference(file);
            await blob.OpenWriteAsync(true);
            return new AzureTextFile(blob, formatProvider, encoding, flushSize);
        }

        /// <summary>
        /// Opens an abstract text file for append operation. Returns the object to work with the file.
        /// </summary>
        /// <param name="file">Abstract file descriptor</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="encoding">Optional file encoding</param>
        /// <param name="flushSize">
        ///     The flush size to set in Kbytes. If zero or less, the default storage provider flush
        ///     size is used.
        /// </param>
        /// <returns>
        /// The object that provides operations to work with the text file.
        /// </returns>
        public async Task<IAbstractTextFile> AppendTextAsync(AbstractFileDescriptor file, IFormatProvider formatProvider = null,
            Encoding encoding = null, int flushSize = 0)
        {
            await EnsureContainerAsync(file.RootContainer);
            var blob = await GetAppendBlobReference(file);
            await blob.OpenWriteAsync(false);
            return new AzureTextFile(blob, formatProvider, encoding, flushSize);
        }

        /// <summary>
        /// Operns an abstract text file for append operation, or creates it, provided, it does not exists.
        /// Returns the object to work with the file.
        /// </summary>
        /// <param name="file">Abstract file descriptor</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="encoding">Optional file encoding</param>
        /// <param name="flushSize">
        ///     The flush size to set in Kbytes. If zero or less, the default storage provider flush
        ///     size is used.
        /// </param>
        /// <returns>
        /// The object that provides operations to work with the text file.
        /// </returns>
        public async Task<IAbstractTextFile> CreateOrAppendTextAsync(AbstractFileDescriptor file, IFormatProvider formatProvider = null,
            Encoding encoding = null, int flushSize = 0)
        {
            return (await ExistsAsync(file))
                ? await AppendTextAsync(file, formatProvider, encoding, flushSize)
                : await CreateTextAsync(file, formatProvider, encoding, flushSize);
        }

        /// <summary>
        /// Gets the Azure blob name from an abstract file descriptor
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public static string GetBlobNameFromAbstractFile(AbstractFileDescriptor descriptor)
        {
            var folder = "";
            if (descriptor.PathSegments != null)
            {
                folder = string.Join("//", descriptor.PathSegments);
            }
            return Path.Combine(folder, descriptor.FileName);
        }

        /// <summary>
        /// Gets a reference to the block blob specified in <paramref name="file"/>
        /// </summary>
        /// <param name="file">File descriptor</param>
        /// <returns>Block blob, if can be obtained; null, if the container does not exist</returns>
        private async Task<CloudAppendBlob> GetAppendBlobReference(AbstractFileDescriptor file)
        {
            var containerName = (file.RootContainer ?? "").ToLower();
            var blobClient = StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            if (!await container.ExistsAsync())
            {
                return null;
            }
            return container.GetAppendBlobReference(GetBlobNameFromAbstractFile(file));
        }
    }
}