using System;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using SeemplestLight.Core.Portable.AbstractFiles;

namespace SeemplestLight.Core.Azure.AbstractFiles
{
    /// <summary>
    /// This class implements an abstract file that works with az Azure append blob
    /// </summary>
    public class AzureTextFile: AbstractTextFileBase
    {
        private readonly CloudAppendBlob _appendBlob;
        private readonly Encoding _encoding;

        /// <summary>
        /// Initializes a new instance of this class with the specified <see cref="System.IO.TextWriter"/>
        /// and optional flush size.
        /// </summary>
        /// <param name="appendBlob">Azure append blob</param>
        /// <param name="formatProvider">Optional format provider</param>
        /// <param name="encoding">Optional file encoding</param>
        /// <param name="flushSize">
        /// The flush size Kbytes. If zero or less, it is set to 4096 KByte.
        /// </param>
        public AzureTextFile(CloudAppendBlob appendBlob, IFormatProvider formatProvider = null, Encoding encoding = null, int flushSize = 0) : 
            base(formatProvider, encoding, flushSize)
        {
            _appendBlob = appendBlob;
            _encoding = encoding ?? Encoding.UTF8;
        }

        /// <summary>
        /// Initializes a new instance of this class with the specified <see cref="System.IO.TextWriter"/>
        /// and optional flush size.
        /// </summary>
        /// <param name="reader">Text reader to access the file</param>
        public AzureTextFile(TextReader reader) : base(reader)
        {
        }

        /// <summary>
        /// Define how to write out the content of this file
        /// </summary>
        /// <param name="content">Text content to write out</param>
        protected override void WriteContent(string content)
        {
            _appendBlob.AppendText(content, _encoding);
        }
    }
}