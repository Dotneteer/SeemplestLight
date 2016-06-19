using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestLight.Net46.Core.Configuration;
using Shouldly;

namespace SeemplestLight.Net46.Core.Test.Configuration
{
    [TestClass]
    public class AppConfigReaderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            AppConfigReader.Reset();
        }

        [TestCleanup]
        public void Cleanup()
        {
            AppConfigReader.Reset();
        }

        [TestMethod]
        public void GetConfigurationValueWorksAsExpected()
        {
            // --- Arrange
            var handler = new AppConfigReader();

            // --- Act
            string value1;
            var found1 = handler.GetConfigurationValue("NonExistingCategory", "key", out value1);
            string value2;
            var found2 = handler.GetConfigurationValue("Category1", "NonExistingKey", out value2);
            string value3;
            var found3 = handler.GetConfigurationValue("Category1", "Key1", out value3);

            // --- Assert
            found1.ShouldBeFalse();
            found2.ShouldBeFalse();
            found3.ShouldBeTrue();
            value3.ShouldBe("123");
        }

        [TestMethod]
        public void GetConfigurationValueWorksWithMachineSection()
        {
            // --- Arrange
            var handler = new AppConfigReader();

            // --- Act
            AppConfigReader.PretendMachine("TestMachine");
            string value1;
            var found1 = handler.GetConfigurationValue("NonExistingCategory", "key", out value1);
            string value2;
            var found2 = handler.GetConfigurationValue("Category1", "NonExistingKey", out value2);
            string value3;
            var found3 = handler.GetConfigurationValue("Category1", "Key2", out value3);
            AppConfigReader.PretendMachine("StagingMachine");
            string value4;
            var found4 = handler.GetConfigurationValue("Category1", "Key2", out value4);

            AppConfigReader.PretendMachine("Unknown");
            string value5;
            var found5 = handler.GetConfigurationValue("Category1", "Key2", out value5);

            // --- Assert
            found1.ShouldBeFalse();
            found2.ShouldBeFalse();
            found3.ShouldBeTrue();
            value3.ShouldBe("Hello-Test");
            found4.ShouldBeTrue();
            value4.ShouldBe("Hello-Staging");
            found5.ShouldBeTrue();
            value5.ShouldBe("Hello");
        }
    }
}