using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace SeemplestLight.Core.Portable.AbstractFiles
{
    /// <summary>
    /// This class implements a text file that works on the Windows file system.
    /// </summary>
    public abstract class AbstractTextFileBase: IAbstractTextFile
    {
        /// <summary>
        /// The <see cref="TextWriter"/> to write to the file.
        /// </summary>
        public TextWriter Writer { get; }

        /// <summary>
        /// The <see cref="TextReader"/> to read from the file
        /// </summary>
        public TextReader Reader { get; }

        /// <summary>
        /// Initializes a new instance of this class with the specified <see cref="TextWriter"/>
        /// and optional flush size.
        /// </summary>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="encoding">Optional file encoding</param>
        /// <param name="flushSize">
        /// The flush size Kbytes. If zero or less, it is set to 4096 KByte.
        /// </param>
        protected AbstractTextFileBase(IFormatProvider formatProvider = null, 
            Encoding encoding = null,  int flushSize = 0)
        {
            if (flushSize <= 0) flushSize = 4096;
            Writer = new AutoFlushTextWriter(formatProvider ?? CultureInfo.InvariantCulture, 
                encoding ?? Encoding.UTF8,
                WriteContent, 
                flushSize);
            Reader = null;
        }

        /// <summary>
        /// Initializes a new instance of this class with the specified <see cref="TextWriter"/>
        /// and optional flush size.
        /// </summary>
        /// <param name="reader">Text reader to access the file</param>
        protected AbstractTextFileBase(TextReader reader)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Writer = null;
        }

        /// <summary>
        /// Define how to write out the content of this file
        /// </summary>
        /// <param name="content">Text content to write out</param>
        protected abstract void WriteContent(string content);

        /// <summary>
        /// Immediately executes a flush operation
        /// </summary>
        public virtual void Flush()
        {
            Writer?.Flush();
        }

        /// <summary>
        /// Closes the file, flushes the unflushed contents
        /// </summary>
        public virtual void Close()
        {
            Flush();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            Close();
            Writer?.Dispose();
            Reader?.Dispose();
        }
    }
}