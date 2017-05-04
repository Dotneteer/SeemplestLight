using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SeemplestLight.Core.Portable.AbstractFiles
{
    /// <summary>
    /// This TextWriter class automatically flushes the text contents
    /// written to the stream
    /// </summary>
    public class AutoFlushTextWriter: TextWriter
    {
        private readonly Action<string> _flushAction;
        private StringWriter _writer;
        private readonly IFormatProvider _formatProvider;
        public int FlushSize { get; }

        /// <summary>
        /// The number of flush check operations invoked
        /// </summary>
        public int FlushCheckCount { get; private set; }

        /// <summary>
        /// The number of Flush operations invoked
        /// </summary>
        public int FlushCount { get; private set; }

        public AutoFlushTextWriter(IFormatProvider formatProvider, Encoding encoding, Action<string> flushAction, int flushSize) 
            : base(formatProvider)
        {
            _flushAction = flushAction;
            _formatProvider = formatProvider;
            Encoding = encoding;
            _writer = new StringWriter(_formatProvider);
            FlushSize = flushSize * 1024;
            if (FlushSize <= 0)
            {
                FlushSize = 1024 * 1024;
            }
        }

        public override void Write(char value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(bool value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(char[] buffer)
        {
            _writer.Write(buffer);
            AutoFlush();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _writer.Write(buffer, index, count);
            AutoFlush();
        }

        public override void Write(decimal value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(double value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(int value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(long value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(object value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(float value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(string value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(string format, params object[] arg)
        {
            _writer.Write(format, arg);
            AutoFlush();
        }

        public override void Write(uint value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override void Write(ulong value)
        {
            _writer.Write(value);
            AutoFlush();
        }

        public override async Task WriteAsync(char value)
        {
            await _writer.WriteAsync(value);
            AutoFlush();
        }

        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            await _writer.WriteAsync(buffer, index, count);
            AutoFlush();
        }

        public override async Task WriteAsync(string value)
        {
            await _writer.WriteAsync(value);
            AutoFlush();
        }

        public override Encoding Encoding { get; }

        /// <summary>
        /// Automatically flushes the contents of the current writer
        /// </summary>
        private void AutoFlush()
        {
            FlushCheckCount++;
            var contents = _writer.ToString();
            if (contents.Length >= FlushSize)
            {
                FlushCount++;
                _flushAction?.Invoke(contents);
                _writer = new StringWriter(_formatProvider);
            }
        }
    }
}