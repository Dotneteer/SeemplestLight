using System;
using System.IO;
using System.Text;
using SeemplestLight.Core.Portable.AbstractFiles;

namespace SeemplestLight.Core.AbstractFiles
{
    /// <summary>
    /// This class implements an abstract fiel that works with the Windows file system
    /// </summary>
    public class WindowsTextFile: AbstractTextFileBase
    {
        private readonly TextWriter _writer;

        /// <summary>
        /// Initializes a new instance of this class with the specified <see cref="System.IO.TextWriter"/>
        /// and optional flush size.
        /// </summary>
        /// <param name="writer">Text writer to access the file</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="encoding">Optional file encoding</param>
        /// <param name="flushSize">
        /// The flush size Kbytes. If zero or less, it is set to 4096 KByte.
        /// </param>
        public WindowsTextFile(TextWriter writer, IFormatProvider formatProvider = null, Encoding encoding = null, int flushSize = 0) 
            : base(formatProvider, encoding, flushSize)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));

        }

        /// <summary>
        /// Initializes a new instance of this class with the specified <see cref="System.IO.TextWriter"/>
        /// and optional flush size.
        /// </summary>
        /// <param name="reader">Text reader to access the file</param>
        public WindowsTextFile(TextReader reader) : base(reader)
        {
        }

        /// <summary>
        /// Define how to write out the content of this file
        /// </summary>
        /// <param name="content">Text content to write out</param>
        protected override void WriteContent(string content)
        {
            _writer.Write(content);
            _writer.Flush();
        }

        /// <summary>
        /// Immediately executes a flush operation
        /// </summary>
        public override void Flush()
        {
            _writer?.Flush();
            base.Flush();
        }

        /// <summary>
        /// Closes the file, flushes the unflushed contents
        /// </summary>
        public override void Close()
        {
            base.Close();
            Writer?.Close();
            Reader?.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            _writer?.Dispose();
        }
    }
}