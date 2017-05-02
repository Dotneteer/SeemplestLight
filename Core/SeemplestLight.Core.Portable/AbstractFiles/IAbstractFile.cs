using System.Collections.Generic;

namespace SeemplestLight.Core.Portable.AbstractFiles
{
    /// <summary>
    /// This interface represents an abstract file that can be managed in
    /// either a file system or in cloud storage.
    /// </summary>
    public interface IAbstractFile
    {
        /// <summary>
        /// Information that describes the storage root of the file
        /// </summary>
        IAbstractFileStorage StorageRoot { get; }
        
        /// <summary>
        /// Root folder/container of the storage system
        /// </summary>
        string RootContainer { get; }

        /// <summary>
        /// Segments of the path under the root folder, taken into 
        /// account as the part of the full path
        /// </summary>
        IEnumerable<string> PathSegments { get; }

        /// <summary>
        /// The name of the file
        /// </summary>
        string FileName { get; }
    }
}