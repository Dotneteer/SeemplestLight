using System.IO;

namespace SeemplestLight.Core.Portable.AbstractFiles
{
    /// <summary>
    /// This interface represents and abstract text file that can be written to an abstract 
    /// file storage.
    /// </summary>
    public interface IAbstractTextFileWriter
    {
        /// <summary>
        /// The <see cref="TextWriter"/> to write to the file.
        /// </summary>
        TextWriter Writer { get; }

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