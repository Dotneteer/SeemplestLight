using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestLight.Core.AbstractFiles;
using SeemplestLight.Core.Portable.AbstractFiles;
using Shouldly;
// ReSharper disable ObjectCreationAsStatement

namespace SeemplestLight.Core.Test.AbstractFiles
{
    [TestClass]
    public class WindowsFileStorageTest
    {
        private const string ROOT = @"C:\Temp\AbstractFiles";

        [TestInitialize]
        public void TestInitialize()
        {
            if (!Directory.Exists(ROOT))
            {
                Directory.CreateDirectory(ROOT);
            }
            var dirInfo = new DirectoryInfo(ROOT);
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                fileInfo.Delete();
            }
            foreach (var folderInfo in dirInfo.GetDirectories())
            {
                folderInfo.Delete(true);
            }
        }

        [TestMethod]
        public void ConstructionWorksAsExpected()
        {
            // --- Act
            var wfs = new WindowsFileStorage(ROOT);

            // --- Assert
            wfs.RootFolder.ShouldBe(ROOT);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructionFailsWithNull()
        {
            // --- Act
            new WindowsFileStorage(null);
        }

        [TestMethod]
        public async Task CreateContainerWorksWithNewName()
        {
            // --- Arrange
            const string CONTAINER = "Container1";
            var wfs = new WindowsFileStorage(ROOT);
            var before = await wfs.ContainerExistsAsync(CONTAINER);
            // --- Act

            await wfs.CreateContainerAsync("Container1");

            // --- Assert
            before.ShouldBeFalse();
            (await wfs.ContainerExistsAsync(CONTAINER)).ShouldBeTrue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task CreateContainerFailsWithExistingName()
        {
            // --- Arrange
            const string CONTAINER = "Container1";
            var wfs = new WindowsFileStorage(ROOT);
            await wfs.CreateContainerAsync(CONTAINER);
            
            // --- Act
            await wfs.CreateContainerAsync("Container1");
        }

        [TestMethod]
        public async Task GetContainersWorksWithNoContainer()
        {
            // --- Arrange
            var wfs = new WindowsFileStorage(ROOT);

            // --- Act
            var containers = await wfs.GetContainersAsync();

            // --- Assert
            containers.Count.ShouldBe(0);
        }

        [TestMethod]
        public async Task GetContainersWorksWithExistingContainers()
        {
            // --- Arrange
            var wfs = new WindowsFileStorage(ROOT);
            await wfs.CreateContainerAsync("Container1");
            await wfs.CreateContainerAsync("Container2");
            await wfs.CreateContainerAsync("Container3");

            // --- Act
            var containers = await wfs.GetContainersAsync();

            // --- Assert
            containers.Count.ShouldBe(3);
        }

        [TestMethod]
        public async Task EnsureContainerWorksWithNonExistingContainer()
        {
            // --- Arrange
            const string CONTAINER = "NonExisting";
            var wfs = new WindowsFileStorage(ROOT);
            await wfs.CreateContainerAsync("Container1");

            // --- Act
            await wfs.EnsureContainerAsync(CONTAINER);

            // --- Assert
            (await wfs.ContainerExistsAsync(CONTAINER)).ShouldBeTrue();
        }

        [TestMethod]
        public async Task EnsureContainerWorksWithExistingContainer()
        {
            // --- Arrange
            const string CONTAINER = "Existing";
            var wfs = new WindowsFileStorage(ROOT);
            await wfs.CreateContainerAsync(CONTAINER);

            // --- Act
            await wfs.EnsureContainerAsync(CONTAINER);

            // --- Assert
            (await wfs.ContainerExistsAsync(CONTAINER)).ShouldBeTrue();
        }

        [TestMethod]
        public async Task RemoveContainerWorksWithNonExistingContainer()
        {
            // --- Arrange
            const string CONTAINER = "NonExisting";
            var wfs = new WindowsFileStorage(ROOT);
            await wfs.CreateContainerAsync("Container1");

            // --- Act
            var removed = await wfs.RemoveContainerAsync(CONTAINER);

            // --- Assert
            removed.ShouldBeFalse();
            (await wfs.ContainerExistsAsync(CONTAINER)).ShouldBeFalse();
        }

        [TestMethod]
        public async Task RemoveContainerWorksWithEmptyContainer()
        {
            // --- Arrange
            const string CONTAINER = "EmptyContainer";
            var wfs = new WindowsFileStorage(ROOT);
            await wfs.CreateContainerAsync("Container1");
            await wfs.CreateContainerAsync(CONTAINER);

            // --- Act
            var removed = await wfs.RemoveContainerAsync(CONTAINER);

            // --- Assert
            removed.ShouldBeTrue();
            (await wfs.ContainerExistsAsync(CONTAINER)).ShouldBeFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task RemoveContainerFailsWithNonEmptyContainer()
        {
            // --- Arrange
            const string CONTAINER = "NonEmptyContainer";
            var wfs = new WindowsFileStorage(ROOT);
            await wfs.CreateContainerAsync("Container1");
            await wfs.CreateContainerAsync(CONTAINER);
            var filePath = Path.Combine(ROOT, Path.Combine(CONTAINER), "file.txt");
            File.WriteAllText(filePath, "TextContents");

            // --- Act
            var removed = await wfs.RemoveContainerAsync(CONTAINER);

            // --- Assert
            removed.ShouldBeTrue();
            (await wfs.ContainerExistsAsync(CONTAINER)).ShouldBeFalse();
        }

        [TestMethod]
        public async Task DeleteWorksWithExistingFile()
        {
            // --- Arrange
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            using (var textFile = await wfs.CreateTextAsync(file))
            {
                textFile.Writer.Write("Awesome");
            }

            // --- Act
            var deleted = await wfs.DeleteAsync(file);

            // --- Assert
            deleted.ShouldBeTrue();
            (await wfs.ExistsAsync(file)).ShouldBeFalse();
        }

        [TestMethod]
        public async Task DeleteWorksWithNonExistingFile()
        {
            // --- Arrange
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            await wfs.DeleteAsync(file);

            // --- Act
            var deleted = await wfs.DeleteAsync(file);

            // --- Assert
            deleted.ShouldBeFalse();
            (await wfs.ExistsAsync(file)).ShouldBeFalse();
        }

        [TestMethod]
        public async Task CreateTextWorksAsExpected()
        {
            // --- Arrange
            const string BODY = "This is a text file";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");

            // --- Act
            using (var textFile = await wfs.CreateTextAsync(file))
            {
                textFile.Writer.Write(BODY);
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task CreateTextWorksWithUnicode()
        {
            // --- Arrange
            const string BODY = "This is a text file";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");

            // --- Act
            using (var textFile = await wfs.CreateTextAsync(file, encoding: Encoding.Unicode))
            {
                textFile.Writer.Write(BODY);
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task CreateTextWorksWithUtf32Encoding()
        {
            // --- Arrange
            const string BODY = "This is a text file";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");

            // --- Act
            using (var textFile = await wfs.CreateTextAsync(file, encoding: Encoding.UTF32))
            {
                textFile.Writer.Write(BODY);
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task CreateTextWorksWithNeutralCulture()
        {
            // --- Arrange
            const string BODY = "1.25";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");

            // --- Act
            using (var textFile = await wfs.CreateTextAsync(file))
            {
                textFile.Writer.Write(1.25);
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task CreateTextWorksWithSpecificCulture()
        {
            // --- Arrange
            const string BODY = "1,25";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");

            // --- Act
            using (var textFile = await wfs.CreateTextAsync(file, new CultureInfo("hu-hu")))
            {
                textFile.Writer.Write(1.25);
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task AppendTextWorksAsExpected()
        {
            // --- Arrange
            const string BODY = "FirstSecond";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            using (var textFile = await wfs.CreateTextAsync(file))
            {
                textFile.Writer.Write("First");
            }

            // --- Act
            using (var textFile = await wfs.AppendTextAsync(file))
            {
                textFile.Writer.Write("Second");
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task AppendTextWorksWithUnicode()
        {
            // --- Arrange
            const string BODY = "FirstSecond";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            using (var textFile = await wfs.CreateTextAsync(file, encoding: Encoding.Unicode))
            {
                textFile.Writer.Write("First");
            }

            // --- Act
            using (var textFile = await wfs.AppendTextAsync(file, encoding: Encoding.Unicode))
            {
                textFile.Writer.Write("Second");
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task AppendTextWorksWithUtf32()
        {
            // --- Arrange
            const string BODY = "FirstSecond";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            using (var textFile = await wfs.CreateTextAsync(file, encoding: Encoding.UTF32))
            {
                textFile.Writer.Write("First");
            }

            // --- Act
            using (var textFile = await wfs.AppendTextAsync(file, encoding: Encoding.UTF32))
            {
                textFile.Writer.Write("Second");
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task AppendTextWorksWithNeutralCulture()
        {
            // --- Arrange
            const string BODY = "1.25-1.25";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            using (var textFile = await wfs.CreateTextAsync(file))
            {
                textFile.Writer.Write(1.25);
            }

            // --- Act
            using (var textFile = await wfs.AppendTextAsync(file))
            {
                textFile.Writer.Write(-1.25);
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task AppendTextWorksWithSpecificCulture()
        {
            // --- Arrange
            const string BODY = "1,25-1,25";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            using (var textFile = await wfs.CreateTextAsync(file, new CultureInfo("hu-hu")))
            {
                textFile.Writer.Write(1.25);
            }

            // --- Act
            using (var textFile = await wfs.AppendTextAsync(file, new CultureInfo("hu-hu")))
            {
                textFile.Writer.Write(-1.25);
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task CreateOrAppendTextWorksWithNonExistingFile()
        {
            // --- Arrange
            const string BODY = "This is a text file";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            var fileName = WindowsFileStorage.FilePathFromAbstractFile(file);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // --- Act
            using (var textFile = await wfs.CreateOrAppendTextAsync(file))
            {
                textFile.Writer.Write(BODY);
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }

        [TestMethod]
        public async Task CreateOrAppendTextWorksWithExistingFile()
        {
            // --- Arrange
            const string BODY = "FirstSecond";
            var wfs = new WindowsFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            var fileName = WindowsFileStorage.FilePathFromAbstractFile(file);
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (var textFile = await wfs.CreateTextAsync(file))
            {
                textFile.Writer.Write("First");
            }

            // --- Act
            using (var textFile = await wfs.CreateOrAppendTextAsync(file))
            {
                textFile.Writer.Write("Second");
            }

            // --- Assert
            using (var savedFile = await wfs.OpenTextAsync(file))
            {
                var text = savedFile.Reader.ReadToEnd();
                text.ShouldBe(BODY);
            }
        }
    }
}
