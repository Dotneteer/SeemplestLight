using System;
using System.Text;

namespace SeemplestLight.Core.Portable.AbstractFiles
{
    /// <summary>
    /// This class implements the operations of a service that can work with abstract files.
    /// </summary>
    public class AbstractFileService : IAbstractFileService
    {
        /// <summary>
        /// Creates an abstract text file. Returns the object to work with the file.
        /// </summary>
        /// <param name="file">Abstract file descriptor</param>
        /// <param name="formatProvider"></param>
        /// <param name="encoding"></param>
        /// <param name="flushSize">
        ///     The flush size to set in Kbytes. If zero or less, the default storage provider flush
        ///     size is used.
        /// </param>
        /// <returns>
        /// The object that provides operations to work with the text file.
        /// </returns>
        public IAbstractTextFile CreateText(AbstractFileDescriptor file, IFormatProvider formatProvider = null, Encoding encoding = null, int flushSize = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Opens an abstract text file for append operation. Returns the object to work with the file.
        /// </summary>
        /// <param name="file">Abstract file descriptor</param>
        /// <param name="formatProvider"></param>
        /// <param name="encoding"></param>
        /// <param name="flushSize">
        ///     The flush size to set in Kbytes. If zero or less, the default storage provider flush
        ///     size is used.
        /// </param>
        /// <returns>
        /// The object that provides operations to work with the text file.
        /// </returns>
        public IAbstractTextFile AppendText(AbstractFileDescriptor file, IFormatProvider formatProvider = null, Encoding encoding = null, int flushSize = 0)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Operns an abstract text file for append operation, or creates it, provided, it does not exists.
        /// Returns the object to work with the file.
        /// </summary>
        /// <param name="file">Abstract file descriptor</param>
        /// <param name="formatProvider"></param>
        /// <param name="encoding"></param>
        /// <param name="flushSize">
        ///     The flush size to set in Kbytes. If zero or less, the default storage provider flush
        ///     size is used.
        /// </param>
        /// <returns>
        /// The object that provides operations to work with the text file.
        /// </returns>
        public IAbstractTextFile CreateOrAppendText(AbstractFileDescriptor file, IFormatProvider formatProvider = null, Encoding encoding = null, int flushSize = 0)
        {
            throw new System.NotImplementedException();
        }
    }
}