using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public Task<List<string>> GetContainersAsync()
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
        public Task<bool> ContainerExistsAsync(string containerName)
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
        public async Task CreateContainerAsync(string containerName)
        {
            if (await ContainerExistsAsync(containerName))
            {
                throw new InvalidOperationException($"{containerName} container already exists, it cannot be created.");
            }
            Directory.CreateDirectory(Path.Combine(RootFolder, containerName));
        }

        /// <summary>
        /// Creates the specified container in the storage, provided it does not exist yet
        /// </summary>
        /// <param name="containerName">Name of the new container</param>
        public async Task EnsureContainerAsync(string containerName)
        {
            await Task.Run(() =>
            {
                var dirName = Path.Combine(RootFolder, containerName);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
            });
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
        public Task<bool> RemoveContainerAsync(string containerName, bool eraseContents = false)
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
                Directory.Delete(containerPath, eraseContents);
                return true;
            });
        }

        /// <summary>
        /// Checks whether the specified file exists in the storage.
        /// </summary>
        /// <param name="file">File to check</param>
        /// <returns>True, if the file exists; otherwise, false.</returns>
        public Task<bool> ExistsAsync(AbstractFileDescriptor file)
        {
            return Task.Run(() => File.Exists(GetFilePath(file)));
        }

        /// <summary>
        /// Removes the specified abstract file
        /// </summary>
        /// <param name="file">File to remove</param>
        /// <returns>True, if the file existed before the remove operation</returns>
        public Task<bool> DeleteAsync(AbstractFileDescriptor file)
        {
            return Task.Run(() =>
            {
                var path = GetFilePath(file);
                var exists = File.Exists(path);
                if (exists)
                {
                    File.Delete(path);
                }
                return exists;
            });
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
            return await Task.Run(() =>
            {
                var textFile = File.Open(GetFilePath(file), FileMode.Open, FileAccess.Read);
                var reader = encoding == null 
                ? new StreamReader(textFile)
                : new StreamReader(textFile, encoding);
                return new WindowsTextFile(reader);
            });
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
            return await Task.Run(() =>
            {
                var fileName = GetFilePath(file);
                var dir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dir))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Directory.CreateDirectory(dir);
                }
                var textFile = File.Open(GetFilePath(file), FileMode.Create);
                var writer = encoding == null
                    ? new StreamWriter(textFile)
                    : new StreamWriter(textFile, encoding);
                return new WindowsTextFile(writer, formatProvider, encoding, flushSize);
            });
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
            return await Task.Run(() =>
            {
                var textFile = File.Open(GetFilePath(file), FileMode.Append);
                var writer = encoding == null
                    ? new StreamWriter(textFile)
                    : new StreamWriter(textFile, encoding);
                return new WindowsTextFile(writer, formatProvider, encoding, flushSize);
            });
        }

        /// <summary>
        /// Opens an abstract text file for append operation, or creates it, provided, it does not exists.
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
        public async Task<IAbstractTextFile> CreateOrAppendTextAsync(AbstractFileDescriptor file,
            IFormatProvider formatProvider = null,
            Encoding encoding = null, int flushSize = 0)
        {
            return (await ExistsAsync(file))
                ? await AppendTextAsync(file, formatProvider, encoding, flushSize)
                : await CreateTextAsync(file, formatProvider, encoding, flushSize);
        }

        /// <summary>
        /// Creates a Windows path from an <see cref="AbstractFileDescriptor"/>
        /// </summary>
        /// <param name="descriptor">File descriptior to create the path from</param>
        /// <returns>Windows file path</returns>
        public static string FilePathFromAbstractFile(AbstractFileDescriptor descriptor)
        {
            var folder = descriptor.RootContainer ?? "";
            if (descriptor.PathSegments != null)
            {
                folder = descriptor.PathSegments.Aggregate(folder, Path.Combine);
            }
            return Path.Combine(folder, descriptor.FileName);
        }

        /// <summary>
        /// Gets the full file path from an abstract descriptor
        /// </summary>
        private string GetFilePath(AbstractFileDescriptor descriptor) =>
            Path.Combine(RootFolder, FilePathFromAbstractFile(descriptor));
    }
}