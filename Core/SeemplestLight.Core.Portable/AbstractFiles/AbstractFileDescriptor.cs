using System;
using System.Collections.Generic;

namespace SeemplestLight.Core.Portable.AbstractFiles
{
    /// <summary>
    /// This interface represents an abstract file that can be managed in
    /// either a file system or in cloud storage.
    /// </summary>
    public class AbstractFileDescriptor
    {
        /// <summary>
        /// Root folder/container of the storage system
        /// </summary>
        public string RootContainer { get; }

        /// <summary>
        /// Segments of the path under the root folder, taken into 
        /// account as the part of the full path
        /// </summary>
        public IEnumerable<string> PathSegments { get; }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Creates an abstract file from the specified parts
        /// </summary>
        /// <param name="rootContainer">Root folder name</param>
        /// <param name="pathSegments">File path segments</param>
        /// <param name="fileName">File name with extension</param>
        public AbstractFileDescriptor(string rootContainer, IEnumerable<string> pathSegments, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name cannot be null, empty, or whitespace only", nameof(fileName));
            }
            RootContainer = rootContainer;
            PathSegments = pathSegments ?? new string[0];
            FileName = fileName;
        }
    }
}