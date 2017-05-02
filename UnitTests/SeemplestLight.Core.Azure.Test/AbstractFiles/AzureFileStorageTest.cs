using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestLight.Core.Azure.AbstractFiles;
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
            var before = await afs.ContainerExists(CONTAINER);
            // --- Act

            await afs.CreateContainer("Container1");

            // --- Assert
            before.ShouldBeFalse();
            (await afs.ContainerExists(CONTAINER)).ShouldBeTrue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task CreateContainerFailsWithExistingName()
        {
            // --- Arrange
            const string CONTAINER = "Container2";
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainer(CONTAINER);

            // --- Act
            await afs.CreateContainer("Container2");
        }

        [TestMethod]
        public async Task GetContainersWorksWithNoContainer()
        {
            // --- Arrange
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            RemoveContainers();

            // --- Act
            var containers = await afs.GetContainers();

            // --- Assert
            containers.Count.ShouldBe(0);
        }

        [TestMethod]
        public async Task GetContainersWorksWithExistingContainers()
        {
            // --- Arrange
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainer("Container3");
            await afs.CreateContainer("Container4");

            // --- Act
            var containers = await afs.GetContainers();

            // --- Assert
            containers.Count.ShouldBe(2);
        }

        [TestMethod]
        public async Task RemoveContainerWorksWithNonExistingContainer()
        {
            // --- Arrange
            const string CONTAINER = "NonExisting";
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainer("Container5");

            // --- Act
            var removed = await afs.RemoveContainer(CONTAINER);

            // --- Assert
            removed.ShouldBeFalse();
            (await afs.ContainerExists(CONTAINER)).ShouldBeFalse();
        }

        [TestMethod]
        public async Task RemoveContainerWorksWithEmptyContainer()
        {
            // --- Arrange
            const string CONTAINER = "EmptyContainer";
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainer("Container6");
            await afs.CreateContainer(CONTAINER);

            // --- Act
            var removed = await afs.RemoveContainer(CONTAINER);

            // --- Assert
            removed.ShouldBeTrue();
            (await afs.ContainerExists(CONTAINER)).ShouldBeFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task RemoveContainerFailsWithNonEmptyContainer()
        {
            // --- Arrange
            const string CONTAINER = "NonEmptyContainer";
            const string FOLDER = @"C:\Temp\AbstractFiles";
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            await afs.CreateContainer("Container7");
            await afs.CreateContainer(CONTAINER);
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
            var removed = await afs.RemoveContainer(CONTAINER);

            // --- Assert
            removed.ShouldBeTrue();
            (await afs.ContainerExists(CONTAINER)).ShouldBeFalse();
        }

        private static void RemoveContainers()
        {
            var afs = new AzureFileStorage(STORAGE_IN_APP_CONFIG);
            Task.WaitAll(
                afs.RemoveContainer("Container1", true),
                afs.RemoveContainer("Container2", true),
                afs.RemoveContainer("Container3", true),
                afs.RemoveContainer("Container4", true),
                afs.RemoveContainer("Container5", true),
                afs.RemoveContainer("Container6", true),
                afs.RemoveContainer("Container7", true),
                afs.RemoveContainer("EmptyContainer", true),
                afs.RemoveContainer("NonEmptyContainer", true));
        }
    }
}
