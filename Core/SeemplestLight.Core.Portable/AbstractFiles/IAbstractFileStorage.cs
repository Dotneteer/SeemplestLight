﻿using System;
using System.Collections.Generic;
using System.Text;
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
        Task<List<string>> GetContainersAsync();

        /// <summary>
        /// Checks whether the container exists within the storage
        /// </summary>
        /// <param name="containerName">Name of the container to check</param>
        /// <returns>True, if the container exists; otherwise, false</returns>
        Task<bool> ContainerExistsAsync(string containerName);

        /// <summary>
        /// Creates a new container within the storage
        /// </summary>
        /// <param name="containerName">Name of the new container</param>
        /// <exception cref="InvalidOperationException">
        /// The container already exists, thus it cannot be created.
        /// </exception>
        Task CreateContainerAsync(string containerName);

        /// <summary>
        /// Creates the specified container in the storage, provided it does not exist yet
        /// </summary>
        /// <param name="containerName">Name of the new container</param>
        Task EnsureContainerAsync(string containerName);

        /// <summary>
        /// Removes a container from the storage, provided, it is empty
        /// </summary>
        /// <param name="containerName">Name of the container to remove</param>
        /// <param name="eraseContents">True indicates that the container should be emptied</param>
        /// <returns>True, if the container has been removed; otherwise, false</returns>
        /// <exception cref="InvalidOperationException">
        /// The container is not empty, thus it cannot be removed.
        /// </exception>
        Task<bool> RemoveContainerAsync(string containerName, bool eraseContents = false);

        /// <summary>
        /// Checks whether the specified file exists in the storage.
        /// </summary>
        /// <param name="file">File to check</param>
        /// <returns>True, if the file exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(AbstractFileDescriptor file);

        /// <summary>
        /// Removes the specified abstract file
        /// </summary>
        /// <param name="file">File to remove</param>
        /// <returns>True, if the file existed before the remove operation</returns>
        Task<bool> DeleteAsync(AbstractFileDescriptor file);

        /// <summary>
        /// Opens a text file for read. Returns the object to work with the file.
        /// </summary>
        /// <param name="file">Abstract file descriptor</param>
        /// <param name="encoding">Optional file encoding</param>
        /// <returns>
        /// The object that provides operations to work with the text file.
        /// </returns>
        Task<IAbstractTextFile> OpenTextAsync(AbstractFileDescriptor file, Encoding encoding = null);

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
        Task<IAbstractTextFile> CreateTextAsync(AbstractFileDescriptor file, IFormatProvider formatProvider = null, Encoding encoding = null, int flushSize = 0);

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
        Task<IAbstractTextFile> AppendTextAsync(AbstractFileDescriptor file, IFormatProvider formatProvider = null, Encoding encoding = null, int flushSize = 0);

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
        Task<IAbstractTextFile> CreateOrAppendTextAsync(AbstractFileDescriptor file, IFormatProvider formatProvider = null, Encoding encoding = null, int flushSize = 0);
    }
}