using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestLight.Core.Portable.AbstractFiles;
using Shouldly;

namespace SeemplestLight.Core.Test.AbstractFiles
{
    [TestClass]
    public class AutopFlushTextWriterTest
    {
        [TestMethod]
        public void ConstructionWorksAsExpected()
        {
            // --- Act
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 812);
            
            // --- Assert
            tw.ShouldNotBeNull();
            tw.FlushSize.ShouldBe(812 * 1024);
            tw.FormatProvider.ShouldBe(CultureInfo.InvariantCulture);
            tw.Encoding.ShouldBe(Encoding.UTF8);
            tw.FlushCount.ShouldBe(0);
            tw.FlushCheckCount.ShouldBe(0);
        }

        [TestMethod]
        public void ConstructionWorksWithWrongInvalidSize()
        {
            // --- Act
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, -123);

            // --- Assert
            tw.ShouldNotBeNull();
            tw.FlushSize.ShouldBe(1024 * 1024);
            tw.FormatProvider.ShouldBe(CultureInfo.InvariantCulture);
            tw.Encoding.ShouldBe(Encoding.UTF8);
            tw.FlushCount.ShouldBe(0);
            tw.FlushCheckCount.ShouldBe(0);
        }

        [TestMethod]
        public void FlushActionIsInvoked()
        {
            // --- Arrange
            var flushed = false;
            var flushedContent = string.Empty;
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8,
                text =>
                {
                    flushed = true;
                    flushedContent = text;
                }, 1);

            // --- Act
            tw.Write(new string('*', 2048));

            // --- Assert
            flushed.ShouldBeTrue();
            flushedContent.ShouldBe(new string('*', 2048));
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(1);
        }

        [TestMethod]
        public void WriteCharWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 1023; i++)
            {
                tw.Write('+');
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write('+');

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(1024);
        }

        [TestMethod]
        public void WriteBoolWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 255; i++)
            {
                tw.Write(true);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(true);

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(256);
        }

        [TestMethod]
        public void WriteCharArrayWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 255; i++)
            {
                tw.Write(new [] {'c', 'h', 'a', 'r'});
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(new[] { 'c', 'h', 'a', 'r' });

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(256);
        }

        [TestMethod]
        public void WriteCharArrayWithIndexWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 255; i++)
            {
                tw.Write(new[] { '*', 'c', 'h', 'a', 'r', '*' }, 1, 4);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(new[] { 'c', 'h', 'a', 'r' });

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(256);
        }

        [TestMethod]
        public void WriteDecimalWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 255; i++)
            {
                tw.Write(1234m);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(1234m);

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(256);
        }

        [TestMethod]
        public void WriteDoubleWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 127; i++)
            {
                tw.Write(-234.125);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(-234.125);

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(128);
        }

        [TestMethod]
        public void WriteIntWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 127; i++)
            {
                tw.Write(-2345678);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(-2345678);

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(128);
        }

        [TestMethod]
        public void WriteLongWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 63; i++)
            {
                tw.Write(-234567812345678);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(-234567812345678);

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(64);
        }

        [TestMethod]
        public void WriteObjectWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 78; i++)
            {
                tw.Write(new object());
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(new object());

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(79);
        }

        [TestMethod]
        public void WriteFloatWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 127; i++)
            {
                tw.Write(-234.125f);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(-234.125f);

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(128);
        }

        [TestMethod]
        public void WriteStringWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 78; i++)
            {
                tw.Write("System.Object");
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write("System.Object");

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(79);
        }

        [TestMethod]
        public void WriteFormattedStringWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 78; i++)
            {
                tw.Write("This is: {0}", 1234);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write("This is: {0}", 1234);

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(79);
        }

        [TestMethod]
        public void WriteUintWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 127; i++)
            {
                tw.Write(12345678u);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(12345678u);

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(128);
        }

        [TestMethod]
        public void WriteUlongWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 63; i++)
            {
                tw.Write(1234567812345678ul);
            }
            var flushCountBefore = tw.FlushCount;
            tw.Write(1234567812345678ul);

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(64);
        }

        [TestMethod]
        public async Task WriteCharAsyncWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 1023; i++)
            {
                await tw.WriteAsync('+');
            }
            var flushCountBefore = tw.FlushCount;
            await tw.WriteAsync('+');

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(1024);
        }

        [TestMethod]
        public async Task WriteAsyncCharArrayWithIndexWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 255; i++)
            {
                await tw.WriteAsync(new[] { '*', 'c', 'h', 'a', 'r', '*' }, 1, 4);
            }
            var flushCountBefore = tw.FlushCount;
            await tw.WriteAsync(new[] { 'c', 'h', 'a', 'r' });

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(256);
        }

        [TestMethod]
        public async Task WriteAsyncStringWorksAsExpected()
        {
            // --- Arrange
            var tw = new AutoFlushTextWriter(CultureInfo.InvariantCulture, Encoding.UTF8, text => { }, 1);

            // --- Act
            for (var i = 0; i < 78; i++)
            {
                await tw.WriteAsync("System.Object");
            }
            var flushCountBefore = tw.FlushCount;
            await tw.WriteAsync("System.Object");

            // --- Assert
            flushCountBefore.ShouldBe(0);
            tw.FlushCount.ShouldBe(1);
            tw.FlushCheckCount.ShouldBe(79);
        }


    }
}
