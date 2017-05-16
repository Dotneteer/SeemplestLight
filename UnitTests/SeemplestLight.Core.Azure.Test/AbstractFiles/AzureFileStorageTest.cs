using System;
using System.Globalization;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestLight.Core.Azure.AbstractFiles;
using SeemplestLight.Core.Portable.AbstractFiles;
using Shouldly;

// ReSharper disable ObjectCreationAsStatement

namespace SeemplestLight.Core.Azure.Test.AbstractFiles
{
    [TestClass]
    public class AzureFileStorageTest
    {
        private const string STORAGE_IN_APP_CONFIG = "appSettingsKey=TestStorage";
        private const string STORAGE_ACCOUNT_INFO = "DefaultEndpointsProtocol=https;AccountName=seemplestlighttest;AccountKey=YFFfxSGnq0Yx3xuERrmmZWiewijwyirRobc5U/JXbU3gS9EXoXGweBJ/XzkEGjPtfQA5AhqEw6aKQ6tfEkrTMw==;EndpointSuffix=core.windows.net";
        private const string STORAGE_NAME = "seemplestlighttest";
        private const string ROOT = "appSettingsKey=TestStorage";

        [ClassCleanup]
        public static void ClassCleanup()
        {
            RemoveContainers();
        }

        [TestMethod]
        public void ConstructionWorksWithAppConfig()
        {
            // --- Act
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);

            // --- Assert
            afs.StorageAccount.ShouldNotBeNull();
            afs.StorageAccount.Credentials.AccountName.ShouldBe(STORAGE_NAME);
        }

        [TestMethod]
        public void ConstructionWorksWithAccountCredentials()
        {
            // --- Act
            var afs = new AzureFileStorage(STORAGE_ACCOUNT_INFO);

            // --- Assert
            afs.StorageAccount.ShouldNotBeNull();
            afs.StorageAccount.Credentials.AccountName.ShouldBe(STORAGE_NAME);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructionFailsWithNull()
        {
            // --- Act
            new AzureFileStorage(null);
        }

        [TestMethod]
        public async Task CreateContainerWorksWithNewName()
        {
            // --- Arrange
            const string CONTAINER = "Container1";
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            var before = await afs.ContainerExistsAsync(CONTAINER);
            // --- Act

            await afs.CreateContainerAsync("Container1");

            // --- Assert
            before.ShouldBeFalse();
            (await afs.ContainerExistsAsync(CONTAINER)).ShouldBeTrue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task CreateContainerFailsWithExistingName()
        {
            // --- Arrange
            const string CONTAINER = "Container2";
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainerAsync(CONTAINER);

            // --- Act
            await afs.CreateContainerAsync("Container2");
        }

        [TestMethod]
        public async Task GetContainersWorks()
        {
            // --- Arrange
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainerAsync("Container3");
            await afs.CreateContainerAsync("Container4");

            // --- Act
            var containers = await afs.GetContainersAsync();

            // --- Assert
            containers.Count.ShouldBeGreaterThanOrEqualTo(2);
        }

        [TestMethod]
        public async Task RemoveContainerWorksWithNonExistingContainer()
        {
            // --- Arrange
            const string CONTAINER = "NonExisting";
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainerAsync("Container5");

            // --- Act
            var removed = await afs.RemoveContainerAsync(CONTAINER);

            // --- Assert
            removed.ShouldBeFalse();
            (await afs.ContainerExistsAsync(CONTAINER)).ShouldBeFalse();
        }

        [TestMethod]
        public async Task RemoveContainerWorksWithEmptyContainer()
        {
            // --- Arrange
            const string CONTAINER = "EmptyContainer";
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainerAsync("Container6");
            await afs.CreateContainerAsync(CONTAINER);

            // --- Act
            var removed = await afs.RemoveContainerAsync(CONTAINER);

            // --- Assert
            removed.ShouldBeTrue();
            (await afs.ContainerExistsAsync(CONTAINER)).ShouldBeFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task RemoveContainerFailsWithNonEmptyContainer()
        {
            // --- Arrange
            const string CONTAINER = "NonEmptyContainer";
            const string FOLDER = @"C:\Temp\AbstractFiles";
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainerAsync("Container7");
            await afs.CreateContainerAsync(CONTAINER);
            if (!Directory.Exists(FOLDER))
            {
                Directory.CreateDirectory(FOLDER);
            }
            var filePath = Path.Combine(FOLDER, "file.txt");
            File.WriteAllText(filePath, "TextContents");
            var blobClient = afs.StorageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(CONTAINER.ToLower());
            var blockBlob = container.GetBlockBlobReference("myblob1");
            using (var fileStream = File.OpenRead(filePath))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            // --- Act
            var removed = await afs.RemoveContainerAsync(CONTAINER);

            // --- Assert
            removed.ShouldBeTrue();
            (await afs.ContainerExistsAsync(CONTAINER)).ShouldBeFalse();
        }

        [TestMethod]
        public async Task DeleteWorksWithExistingFile()
        {
            // --- Arrange
            var wfs = new AzureFileStorage(ROOT);
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
            var wfs = new AzureFileStorage(ROOT);
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
            var wfs = new AzureFileStorage(ROOT);
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
        public async Task CreateTextWorksWithNeutralCulture()
        {
            // --- Arrange
            const string BODY = "1.25";
            var wfs = new AzureFileStorage(ROOT);
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
            var wfs = new AzureFileStorage(ROOT);
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
            var wfs = new AzureFileStorage(ROOT);
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
            var wfs = new AzureFileStorage(ROOT);
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
            using (var savedFile = await wfs.OpenTextAsync(file, Encoding.Unicode))
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
            var wfs = new AzureFileStorage(ROOT);
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
            using (var savedFile = await wfs.OpenTextAsync(file, Encoding.UTF32))
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
            var wfs = new AzureFileStorage(ROOT);
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
            var wfs = new AzureFileStorage(ROOT);
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
            var wfs = new AzureFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
            await wfs.DeleteAsync(file);

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
            var wfs = new AzureFileStorage(ROOT);
            var file = new AbstractFileDescriptor("Container", null, "TestFile.txt");
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

        private static void RemoveContainers()
        {
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            Task.WaitAll(
                afs.RemoveContainerAsync("Container1", true),
                afs.RemoveContainerAsync("Container2", true),
                afs.RemoveContainerAsync("Container3", true),
                afs.RemoveContainerAsync("Container4", true),
                afs.RemoveContainerAsync("Container5", true),
                afs.RemoveContainerAsync("Container6", true),
                afs.RemoveContainerAsync("Container7", true),
                afs.RemoveContainerAsync("EmptyContainer", true),
                afs.RemoveContainerAsync("NonEmptyContainer", true));
        }
    }
}
