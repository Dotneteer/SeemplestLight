﻿using System;
using System.Globalization;
using System.IO;
using System.Text;
using SeemplestLight.Core.Portable.AbstractFiles;

namespace SeemplestLight.Core.AbstractFiles
{
    /// <summary>
    /// This class implements a text file that works on the Windows file system.
    /// </summary>
    public class WindowsTextFileWriter: IAbstractTextFileWriter
    {
        private readonly TextWriter _writer;

        /// <summary>
        /// The <see cref="TextWriter"/> to write to the file.
        /// </summary>
        public TextWriter Writer { get; }

        /// <summary>
        /// Initializes a new instance of this class with the specified <see cref="TextWriter"/>
        /// and optional flush size.
        /// </summary>
        /// <param name="writer">Text writer to access the file</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="encoding">Optional file encoding</param>
        /// <param name="flushSize">
        /// The flush size Kbytes. If zero or less, it is set to 4096 KByte.
        /// </param>
        public WindowsTextFileWriter(TextWriter writer, IFormatProvider formatProvider = null, 
            Encoding encoding = null,  int flushSize = 0)
        {
            if (flushSize <= 0) flushSize = 4096;
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            Writer = new AutoFlushTextWriter(formatProvider ?? CultureInfo.InvariantCulture, 
                encoding ?? Encoding.UTF8, 
                content => Flush(), 
                flushSize);
        }

        /// <summary>
        /// Immediately executes a flush operation
        /// </summary>
        public void Flush()
        {
            _writer.Flush();
        }

        /// <summary>
        /// Closes the file, flushes the unflushed contents
        /// </summary>
        public void Close()
        {
            _writer.Close();
        }
    }
}