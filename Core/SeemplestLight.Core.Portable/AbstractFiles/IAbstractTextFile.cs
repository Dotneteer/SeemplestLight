using System.IO;

namespace SeemplestLight.Core.Portable.AbstractFiles
{
    /// <summary>
    /// This interface represents and abstract text file that can be written to an abstract 
    /// file storage.
    /// </summary>
    public interface IAbstractTextFile
    {
        /// <summary>
        /// Gets the descriptor of this abstract file
        /// </summary>
        AbstractFileDescriptor FileDescriptor { get; }

        /// <summary>
        /// The <see cref="TextWriter"/> to write to the file.
        /// </summary>
        TextWriter Writer { get; }

        /// <summary>
        /// The number of Kbytes to use to flush the file contents automatically
        /// </summary>
        /// <remarks>
        /// If size is zero or less, the default IAbstractFileStorage flush size is used
        /// </remarks>
        int FlushSize { get; }

        /// <summary>
        /// Immediately executes a flush operation
        /// </summary>
        void Flush();

        /// <summary>
        /// Closes the file, flushes the unflushed contents
        /// </summary>
        void Close();
    }
}