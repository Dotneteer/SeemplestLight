using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestLight.PortableCore.IoC;
using Shouldly;

namespace SeemplestLight.PortableCore.Test.IoC
{
    [TestClass]
    public class ServiceContainerTest
    {
        [TestMethod]
        public void AutoCreationWithFactoryWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register(
                () => new TestClass1(),
                true);

            // --- Assert
            var instances = ServiceRegistry.Default.GetAllInstances<TestClass1>().ToList();
            instances.Count.ShouldBe(1);

            var defaultInstance = ServiceRegistry.Default.GetInstance<TestClass1>();
            instances[0].ShouldBeSameAs(defaultInstance);
        }

        [TestMethod]
        public void AutoCreationWithWithFactoryAndKeyWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register(
                () => new TestClass1(),
                KEY1,
                true);

            // --- Assert
            var instances = ServiceRegistry.Default.GetAllInstances<TestClass1>().ToList();
            instances.Count.ShouldBe(1);

            var defaultInstance = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            instances[0].ShouldBeSameAs(defaultInstance);
        }

        [TestMethod]
        public void AutoCreationWithFactoryForInterfaceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register<ITestClass>(
                () => new TestClass1(),
                true);

            // --- Assert
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>().ToList();
            instances.Count.ShouldBe(1);

            var defaultInstance = ServiceRegistry.Default.GetInstance<ITestClass>();
            instances[0].ShouldBeSameAs(defaultInstance);
        }

        [TestMethod]
        public void AutoCreationWithFactoryForInterfaceAndKeyWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register<ITestClass>(
                () => new TestClass1(),
                KEY1,
                true);

            // --- Assert
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>().ToList();
            instances.Count.ShouldBe(1);

            var defaultInstance = ServiceRegistry.Default.GetInstance<ITestClass>(KEY1);
            instances[0].ShouldBeSameAs(defaultInstance);
        }

        [TestMethod]
        public void AutoCreationWithInterfaceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register<ITestClass, TestClass1>(true);

            // --- Assert
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>().ToList();
            instances.Count.ShouldBe(1);

            var defaultInstance = ServiceRegistry.Default.GetInstance<ITestClass>();
            instances[0].ShouldBeSameAs(defaultInstance);
        }

        [TestMethod]
        public void DelayedCreationWithFactoryWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            TestClass1.Reset();

            // --- Act
            ServiceRegistry.Default.Register(() => new TestClass1());

            // --- Assert
            TestClass1.InstancesCount.ShouldBe(0);
            ServiceRegistry.Default.GetInstance<TestClass1>();
            TestClass1.InstancesCount.ShouldBe(1);
            var instances = ServiceRegistry.Default.GetAllInstances<TestClass1>();
            var instance = ServiceRegistry.Default.GetInstance<TestClass1>();
            instances.ElementAt(0).ShouldBeSameAs(instance);
        }

        [TestMethod]
        public void TestDelayedCreationWithFactoryAndKey()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();
            TestClass1.Reset();

            // --- Act
            ServiceRegistry.Default.Register(
                () => new TestClass1(),
                KEY1);

            // --- Assert
            TestClass1.InstancesCount.ShouldBe(0);
            var instance = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            TestClass1.InstancesCount.ShouldBe(1);
            var instances = ServiceRegistry.Default.GetAllInstances<TestClass1>();
            instances.ElementAt(0).ShouldBeSameAs(instance);
        }

        [TestMethod]
        public void DelayedCreationWithFactoryForInterfaceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            TestClass1.Reset();

            // --- Act
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1());

            // --- Assert
            TestClass1.InstancesCount.ShouldBe(0);
            var instance = ServiceRegistry.Default.GetInstance<ITestClass>();
            TestClass1.InstancesCount.ShouldBe(1);
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>();
            instances.ElementAt(0).ShouldBeSameAs(instance);
        }

        [TestMethod]
        public void DelayedCreationWithFactoryForInterfaceAndKeyWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();
            TestClass1.Reset();

            // --- Act
            ServiceRegistry.Default.Register<ITestClass>(
                () => new TestClass1(),
                KEY1);

            // --- Assert
            TestClass1.InstancesCount.ShouldBe(0);
            var instance = ServiceRegistry.Default.GetInstance<ITestClass>(KEY1);
            TestClass1.InstancesCount.ShouldBe(1);
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>();
            instances.ElementAt(0).ShouldBeSameAs(instance);
        }

        [TestMethod]
        public void DelayedCreationWithInterfaceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            TestClass1.Reset();

            // --- Act
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();

            // --- Assert
            TestClass1.InstancesCount.ShouldBe(0);
            var instance = ServiceRegistry.Default.GetInstance<ITestClass>();
            TestClass1.InstancesCount.ShouldBe(1);
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>();
            instances.ElementAt(0).ShouldBeSameAs(instance);
        }

        [TestMethod]
        public void ContainsClassWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act/Assert
            ServiceRegistry.Default.ContainsCreated<TestClass1>().ShouldBeFalse();
            ServiceRegistry.Default.GetInstance<TestClass1>();
            ServiceRegistry.Default.ContainsCreated<TestClass1>().ShouldBeTrue();
        }

        [TestMethod]
        public void ContainsInstanceWorksAsExpected()
        {
            // --- Arrange 
            const string KEY1 = "My key";
            ServiceRegistry.Default.Reset();
            var instance = new TestClass1();
            ServiceRegistry.Default.Register(() => instance, KEY1);
            ServiceRegistry.Default.Register<TestClass2>();

            // --- Act/Assert
            ServiceRegistry.Default.ContainsCreated<TestClass1>().ShouldBeFalse();
            ServiceRegistry.Default.ContainsCreated<TestClass1>(KEY1).ShouldBeFalse();
            ServiceRegistry.Default.ContainsCreated<TestClass2>().ShouldBeFalse();
            ServiceRegistry.Default.ContainsCreated<TestClass3>().ShouldBeFalse();

            ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);

            ServiceRegistry.Default.ContainsCreated<TestClass1>().ShouldBeTrue();
            ServiceRegistry.Default.ContainsCreated<TestClass1>(KEY1).ShouldBeTrue();
            ServiceRegistry.Default.ContainsCreated<TestClass2>().ShouldBeFalse();
            ServiceRegistry.Default.ContainsCreated<TestClass3>().ShouldBeFalse();

            ServiceRegistry.Default.GetInstance<TestClass2>();

            ServiceRegistry.Default.ContainsCreated<TestClass1>().ShouldBeTrue();
            ServiceRegistry.Default.ContainsCreated<TestClass1>(KEY1).ShouldBeTrue();
            ServiceRegistry.Default.ContainsCreated<TestClass2>().ShouldBeTrue();
            ServiceRegistry.Default.ContainsCreated<TestClass3>().ShouldBeFalse();
        }

        [TestMethod]
        public void ContainsInstanceForKeyWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "My key";
            const string KEY2 = "My key2";
            ServiceRegistry.Default.Reset();
            var instance = new TestClass1();
            ServiceRegistry.Default.Register(() => instance, KEY1);
            ServiceRegistry.Default.Register<TestClass2>();

            // --- Act/Assert
            ServiceRegistry.Default.ContainsCreated<TestClass1>().ShouldBeFalse();
            ServiceRegistry.Default.ContainsCreated<TestClass1>(KEY1).ShouldBeFalse();
            ServiceRegistry.Default.ContainsCreated<TestClass1>(KEY2).ShouldBeFalse();

            ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);

            ServiceRegistry.Default.ContainsCreated<TestClass1>().ShouldBeTrue();
            ServiceRegistry.Default.ContainsCreated<TestClass1>(KEY1).ShouldBeTrue();
            ServiceRegistry.Default.ContainsCreated<TestClass1>(KEY2).ShouldBeFalse();
            ServiceRegistry.Default.ContainsCreated<TestClass2>(KEY1).ShouldBeFalse();
            ServiceRegistry.Default.ContainsCreated<TestClass3>(KEY1).ShouldBeFalse();
        }

        [TestMethod]
        public void ContainsInstanceAfterUnregisterWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestBaseClass>(true);

            // --- Act/Assert
            ServiceRegistry.Default.IsRegistered<TestBaseClass>().ShouldBeTrue();
            ServiceRegistry.Default.ContainsCreated<TestBaseClass>().ShouldBeTrue();

            var instance = ServiceRegistry.Default.GetInstance<TestBaseClass>();
            instance.Remove();

            ServiceRegistry.Default.IsRegistered<TestBaseClass>().ShouldBeTrue();
            ServiceRegistry.Default.ContainsCreated<TestBaseClass>().ShouldBeFalse();
        }

        [TestMethod]
        public void CreationOfMultipleInstancesWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            TestClassForCreationTime.Reset();
            var factoryWasUsed = 0;

            // --- Act/Assert
            ServiceRegistry.Default.Register(
                () =>
                {
                    factoryWasUsed++;
                    return new TestClassForCreationTime();
                });

            TestClassForCreationTime.InstancesCreated.ShouldBe(0);

            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>();
            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>("Key1");
            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>("Key2");

            TestClassForCreationTime.InstancesCreated.ShouldBe(3);
            factoryWasUsed.ShouldBe(3);
        }

        [TestMethod]
        public void CreationTimeForDefaultInstanceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            TestClassForCreationTime.Reset();

            // --- Act/Assert
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.Register<TestClassForCreationTime>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(1);
            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(1);
        }

        [TestMethod]
        public void CreationTimeForNamedInstanceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            TestClassForCreationTime.Reset();

            // --- Act/Assert
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.Register<TestClassForCreationTime>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>("Key1");
            TestClassForCreationTime.InstancesCreated.ShouldBe(1);
            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>("Key2");
            TestClassForCreationTime.InstancesCreated.ShouldBe(2);
            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>("Key1");
            TestClassForCreationTime.InstancesCreated.ShouldBe(2);
        }

        [TestMethod]
        public void CreationTimeWithFactoryWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            TestClassForCreationTime.Reset();

            // --- Act/Assert
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.Register<TestClassForCreationTime>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(1);
            ServiceRegistry.Default.GetInstance<TestClassForCreationTime>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(1);
        }

        [TestMethod]
        public void CreationTimeWithInterfaceForDefaultInstanceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            TestClassForCreationTime.Reset();

            // --- Act/Assert
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.Register<ITestClass, TestClassForCreationTime>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.GetInstance<ITestClass>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(1);
            ServiceRegistry.Default.GetInstance<ITestClass>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(1);
        }

        [TestMethod]
        public void CreationTimeWithInterfaceForNamedInstanceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            TestClassForCreationTime.Reset();

            // --- Act/Assert
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.Register<ITestClass, TestClassForCreationTime>();
            TestClassForCreationTime.InstancesCreated.ShouldBe(0);
            ServiceRegistry.Default.GetInstance<ITestClass>("Key1");
            TestClassForCreationTime.InstancesCreated.ShouldBe(1);
            ServiceRegistry.Default.GetInstance<ITestClass>("Key2");
            TestClassForCreationTime.InstancesCreated.ShouldBe(2);
            ServiceRegistry.Default.GetInstance<ITestClass>("Key1");
            TestClassForCreationTime.InstancesCreated.ShouldBe(2);
        }

        [TestMethod]
        public void IsClassRegisteredWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act/Assert
            ServiceRegistry.Default.IsRegistered<TestClass1>().ShouldBeFalse();
            ServiceRegistry.Default.Register<TestClass1>();
            ServiceRegistry.Default.IsRegistered<TestClass1>().ShouldBeTrue();

            ServiceRegistry.Default.GetInstance<TestClass1>();
            ServiceRegistry.Default.IsRegistered<TestClass1>().ShouldBeTrue();

            ServiceRegistry.Default.Unregister<TestClass1>();
            ServiceRegistry.Default.IsRegistered<TestClass1>().ShouldBeFalse();
        }

        [TestMethod]
        public void IsClassRegisteredWithFactoryWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act/Assert
            var instance = new TestClass1();
            ServiceRegistry.Default.IsRegistered<TestClass1>().ShouldBeFalse();
            ServiceRegistry.Default.Register(() => instance);
            ServiceRegistry.Default.IsRegistered<TestClass1>().ShouldBeTrue();

            ServiceRegistry.Default.GetInstance<TestClass1>();
            ServiceRegistry.Default.IsRegistered<TestClass1>().ShouldBeTrue();

            ServiceRegistry.Default.Unregister<TestClass1>();
            ServiceRegistry.Default.IsRegistered<TestClass1>().ShouldBeFalse();
        }

        [TestMethod]
        public void IsClassRegisteredWithFactoryAndKeyWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "My key 1";
            const string KEY2 = "My key 2";
            ServiceRegistry.Default.Reset();

            // --- Act/Assert
            var instance = new TestClass1();
            ServiceRegistry.Default.IsRegistered<TestClass1>(KEY1).ShouldBeFalse();
            ServiceRegistry.Default.IsRegistered<TestClass1>(KEY2).ShouldBeFalse();

            ServiceRegistry.Default.Register(() => instance, KEY1);
            ServiceRegistry.Default.IsRegistered<TestClass1>(KEY1).ShouldBeTrue();
            ServiceRegistry.Default.IsRegistered<TestClass1>(KEY2).ShouldBeFalse();

            ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            ServiceRegistry.Default.IsRegistered<TestClass1>(KEY1).ShouldBeTrue();

            ServiceRegistry.Default.Unregister<TestClass1>(KEY1);
            ServiceRegistry.Default.IsRegistered<TestClass1>(KEY1).ShouldBeFalse();
            ServiceRegistry.Default.IsRegistered<TestClass1>(KEY2).ShouldBeFalse();
        }

        [TestMethod]
        public void IsInterfaceRegisteredWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act/Assert
            ServiceRegistry.Default.IsRegistered<ITestClass>().ShouldBeFalse();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();
            ServiceRegistry.Default.IsRegistered<ITestClass>().ShouldBeTrue();

            ServiceRegistry.Default.GetInstance<ITestClass>();
            ServiceRegistry.Default.IsRegistered<ITestClass>().ShouldBeTrue();

            ServiceRegistry.Default.Unregister<ITestClass>();
            ServiceRegistry.Default.IsRegistered<ITestClass>().ShouldBeFalse();
        }

        [TestMethod]
        public void RegisterWorksWithMultipleTimes()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();
        }

        [TestMethod]
        public void BuildInstanceWithMultipleConstructorsNotMarkedWithAttributeWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            var property = new TestClass1();
            ServiceRegistry.Default.Register(() => new TestClass6(property));

            // --- Act/Assert
            var instance1 = new TestClass6();
            instance1.ShouldNotBeNull();
            instance1.MyProperty.ShouldBeNull();

            var instance2 = ServiceRegistry.Default.GetInstance<TestClass6>();
            instance2.ShouldNotBeNull();
            instance2.MyProperty.ShouldNotBeNull();
            instance2.MyProperty.ShouldBeSameAs(property);
        }

        [TestMethod]
        public void BuildWithMultipleConstructorsWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            var property = new TestClass1();
            ServiceRegistry.Default.Register<ITestClass>(() => property);
            ServiceRegistry.Default.Register<TestClass5>();

            // --- Act/Assert
            var instance1 = new TestClass5();
            Assert.IsNotNull(instance1);
            Assert.IsNull(instance1.MyProperty);

            var instance2 = ServiceRegistry.Default.GetInstance<TestClass5>();
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance2.MyProperty);
            Assert.AreSame(property, instance2.MyProperty);
        }

        [TestMethod]
        [ExpectedException(typeof (ActivationException))]
        public void BuildWithMultipleConstructorsNotMarkedWithAttributeFails()
        {
            // --- Arrange
            var property = new TestClass1();
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass>(() => property);

            // --- Act
            ServiceRegistry.Default.Register<TestClass6>();
        }

        [TestMethod]
        [ExpectedException(typeof (ActivationException))]
        public void BuildWithPrivateConstructorFails()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register<TestClass7>();
        }

        [TestMethod]
        public void BuildWithStaticConstructorWorks()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register<TestClass8>();
        }

        [TestMethod]
        public void PublicAndInternalConstructorWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register<TestClass9>();
        }

        [TestMethod]
        public void AddingDefaultForClassRegisteredWithFactoryAndKeyWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();
            var instance1 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance1, KEY1);
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act
            var foundInstance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var defaultInstance = ServiceRegistry.Default.GetInstance<TestClass1>();

            // --- Assert
            Assert.AreSame(instance1, foundInstance1);
            Assert.AreNotSame(foundInstance1, defaultInstance);
        }

        [TestMethod]
        public void AddingFactoryAndKeyForClassRegisteredWithDefaultWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();
            var instance1 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance1, KEY1);

            // --- Act
            var defaultInstance = ServiceRegistry.Default.GetInstance<TestClass1>();
            var foundInstance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);

            // --- Assert
            Assert.AreSame(instance1, foundInstance1);
            Assert.AreNotSame(defaultInstance, foundInstance1);
        }

        [TestMethod]
        public void AddingFactoryAndKeyForClassRegisteredWithFactoryWorksAsExpected()
        {
            // --- Arrange
            const string KEY = "key";
            ServiceRegistry.Default.Reset();
            var instance1 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance1);
            var instance2 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance2, KEY);

            // --- Act
            var defaultInstance = ServiceRegistry.Default.GetInstance<TestClass1>();
            var foundInstance2 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY);

            // --- Assert
            Assert.AreSame(instance1, defaultInstance);
            Assert.AreSame(instance2, foundInstance2);
            Assert.AreNotSame(defaultInstance, foundInstance2);
        }

        [TestMethod]
        public void AddingFactoryAndKeyForClassRegisteredWithFactoryAndDifferentKeyWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "key1";
            const string KEY2 = "key2";
            ServiceRegistry.Default.Reset();
            var instance1 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance1, KEY1);
            var instance2 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance2, KEY2);

            // --- Act
            var foundInstance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var foundInstance2 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);

            // --- Assert
            Assert.AreSame(instance1, foundInstance1);
            Assert.AreSame(instance2, foundInstance2);
            Assert.AreNotSame(foundInstance1, foundInstance2);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void AddingFactoryAndKeyForClassRegisteredWithFactoryAndSameKeyFails()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();
            var instance1 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance1, KEY1);
            var instance2 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance2, KEY1);
        }

        [TestMethod]
        public void TestAddingFactoryForClassRegisteredWithFactoryAndKey()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();
            var instance1 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance1, KEY1);
            var instance2 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance2);

            // --- Act
            var foundInstance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var foundInstance2 = ServiceRegistry.Default.GetInstance<TestClass1>();

            // --- Assert
            Assert.AreSame(instance1, foundInstance1);
            Assert.AreSame(instance2, foundInstance2);
            Assert.AreNotSame(foundInstance1, foundInstance2);
        }

        [TestMethod]
        public void GetAllInstancesOfClassWithCreationWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "key1";
            const string KEY2 = "key2";
            const string KEY3 = "key3";
            ServiceRegistry.Default.Reset();
            TestClass1.Reset();
            var instance0 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance0, true);

            // --- Act/Assert
            var instance1 = new TestClass1();
            var instance2 = new TestClass1();
            var instance3 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance1, KEY1, true);
            ServiceRegistry.Default.Register(() => instance2, KEY2, true);
            ServiceRegistry.Default.Register(() => instance3, KEY3);

            Assert.AreEqual(4, TestClass1.InstancesCount);

            var instances = ServiceRegistry.Default.GetAllCreatedInstances<TestClass1>();
            Assert.AreEqual(3, instances.Count());

            instances = ServiceRegistry.Default.GetAllCreatedInstances<TestClass1>();
            ServiceRegistry.Default.GetInstance<TestClass1>(KEY3);

            Assert.AreEqual(4, instances.Count());

            var list = instances.ToList();

            foreach (var instance in instances)
            {
                if (instance == instance0
                    || instance == instance1
                    || instance == instance2
                    || instance == instance3)
                {
                    list.Remove(instance);
                }
                else
                {
                    Assert.Fail();
                }
            }
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void GetAllInstancesWithInstanceWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";
            const string KEY3 = "MyKey3";
            const string KEY4 = "MyKey4";
            const string KEY5 = "MyKey5";
            const string KEY6 = "MyKey6";

            var instanceOriginal1 = new TestClass1();
            var instanceOriginal2 = new TestClass1();
            var instanceOriginal3 = new TestClass1();
            var instanceOriginal4 = new TestClass1();
            var instanceOriginal5 = new TestClass4();
            var instanceOriginal6 = new TestClass4();

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register(() => instanceOriginal1, KEY1);
            ServiceRegistry.Default.Register(() => instanceOriginal2, KEY2);
            ServiceRegistry.Default.Register(() => instanceOriginal3, KEY3);
            ServiceRegistry.Default.Register(() => instanceOriginal4, KEY4);
            ServiceRegistry.Default.Register(() => instanceOriginal5, KEY5);
            ServiceRegistry.Default.Register(() => instanceOriginal6, KEY6);

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);
            var instance3 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY3);
            var instance4 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY4);
            var instance5 = ServiceRegistry.Default.GetInstance<TestClass4>(KEY5);
            var instance6 = ServiceRegistry.Default.GetInstance<TestClass4>(KEY6);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);
            Assert.IsNotNull(instance4);
            Assert.IsNotNull(instance5);
            Assert.IsNotNull(instance6);

            var allInstances = ServiceRegistry.Default.GetAllInstances(typeof (TestClass1)).ToList();
            Assert.AreEqual(4, allInstances.Count());

            foreach (var instance in allInstances)
            {
                Assert.IsNotNull(instance);

                if (instance.Equals(instance1))
                {
                    instance1 = null;
                }

                if (instance.Equals(instance2))
                {
                    instance2 = null;
                }

                if (instance.Equals(instance3))
                {
                    instance3 = null;
                }

                if (instance.Equals(instance4))
                {
                    instance4 = null;
                }

                if (instance.Equals(instance5))
                {
                    instance5 = null;
                }

                if (instance.Equals(instance6))
                {
                    instance6 = null;
                }
            }

            Assert.IsNull(instance1);
            Assert.IsNull(instance2);
            Assert.IsNull(instance3);
            Assert.IsNull(instance4);
            Assert.IsNotNull(instance5);
            Assert.IsNotNull(instance6);
        }

        [TestMethod]
        public void GetAllInstancesWithInstanceGenericWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";
            const string KEY3 = "MyKey3";
            const string KEY4 = "MyKey4";
            const string KEY5 = "MyKey5";
            const string KEY6 = "MyKey6";

            var instanceOriginal1 = new TestClass1();
            var instanceOriginal2 = new TestClass1();
            var instanceOriginal3 = new TestClass1();
            var instanceOriginal4 = new TestClass1();
            var instanceOriginal5 = new TestClass4();
            var instanceOriginal6 = new TestClass4();

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register(() => instanceOriginal1, KEY1);
            ServiceRegistry.Default.Register(() => instanceOriginal2, KEY2);
            ServiceRegistry.Default.Register(() => instanceOriginal3, KEY3);
            ServiceRegistry.Default.Register(() => instanceOriginal4, KEY4);
            ServiceRegistry.Default.Register(() => instanceOriginal5, KEY5);
            ServiceRegistry.Default.Register(() => instanceOriginal6, KEY6);

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);
            var instance3 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY3);
            var instance4 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY4);
            var instance5 = ServiceRegistry.Default.GetInstance<TestClass4>(KEY5);
            var instance6 = ServiceRegistry.Default.GetInstance<TestClass4>(KEY6);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);
            Assert.IsNotNull(instance4);
            Assert.IsNotNull(instance5);
            Assert.IsNotNull(instance6);

            var allInstances = ServiceRegistry.Default.GetAllInstances<TestClass1>().ToList();
            Assert.AreEqual(4, allInstances.Count());

            foreach (var instance in allInstances)
            {
                Assert.IsNotNull(instance);

                if (instance.Equals(instance1))
                {
                    instance1 = null;
                }

                if (instance.Equals(instance2))
                {
                    instance2 = null;
                }

                if (instance.Equals(instance3))
                {
                    instance3 = null;
                }

                if (instance.Equals(instance4))
                {
                    instance4 = null;
                }

                if (instance.Equals(instance5))
                {
                    instance5 = null;
                }

                if (instance.Equals(instance6))
                {
                    instance6 = null;
                }
            }

            Assert.IsNull(instance1);
            Assert.IsNull(instance2);
            Assert.IsNull(instance3);
            Assert.IsNull(instance4);
            Assert.IsNotNull(instance5);
            Assert.IsNotNull(instance6);
        }

        [TestMethod]
        public void GetAllInstancesWithInterfaceWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";
            const string KEY3 = "MyKey3";
            const string KEY4 = "MyKey4";

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY2);
            var instance3 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY3);
            var instance4 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY4);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);
            Assert.IsNotNull(instance4);

            var createdInstances = ServiceRegistry.Default.GetAllCreatedInstances<ITestClass>();
            Assert.AreEqual(4, createdInstances.Count());

            var allInstances = ServiceRegistry.Default.GetAllInstances(typeof (ITestClass));
            Assert.AreEqual(5, allInstances.Count());

            foreach (var instance in allInstances)
            {
                Assert.IsNotNull(instance);

                if (instance.Equals(instance1))
                {
                    instance1 = null;
                }

                if (instance.Equals(instance2))
                {
                    instance2 = null;
                }

                if (instance.Equals(instance3))
                {
                    instance3 = null;
                }

                if (instance.Equals(instance4))
                {
                    instance4 = null;
                }
            }

            Assert.IsNull(instance1);
            Assert.IsNull(instance2);
            Assert.IsNull(instance3);
            Assert.IsNull(instance4);
        }

        [TestMethod]
        public void GetAllInstancesWithInterfaceGenericWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";
            const string KEY3 = "MyKey3";
            const string KEY4 = "MyKey4";

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY2);
            var instance3 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY3);
            var instance4 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY4);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);
            Assert.IsNotNull(instance4);

            var createdInstances = ServiceRegistry.Default.GetAllCreatedInstances<ITestClass>();
            Assert.AreEqual(4, createdInstances.Count());

            var allInstances = ServiceRegistry.Default.GetAllInstances<ITestClass>().ToList();
            Assert.AreEqual(5, allInstances.Count()); // including default instance

            foreach (var instance in allInstances)
            {
                Assert.IsNotNull(instance);

                if (instance.Equals(instance1))
                {
                    instance1 = null;
                }

                if (instance.Equals(instance2))
                {
                    instance2 = null;
                }

                if (instance.Equals(instance3))
                {
                    instance3 = null;
                }

                if (instance.Equals(instance4))
                {
                    instance4 = null;
                }
            }

            Assert.IsNull(instance1);
            Assert.IsNull(instance2);
            Assert.IsNull(instance3);
            Assert.IsNull(instance4);
        }

        [TestMethod]
        public void GetAllInstancesWithTypeWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";
            const string KEY3 = "MyKey3";
            const string KEY4 = "MyKey4";
            const string KEY5 = "MyKey5";
            const string KEY6 = "MyKey6";

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();
            ServiceRegistry.Default.Register<TestClass2>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance(typeof (TestClass1), KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance(typeof (TestClass1), KEY2);
            var instance3 = ServiceRegistry.Default.GetInstance(typeof (TestClass1), KEY3);
            var instance4 = ServiceRegistry.Default.GetInstance(typeof (TestClass1), KEY4);
            var instance5 = ServiceRegistry.Default.GetInstance(typeof (TestClass2), KEY5);
            var instance6 = ServiceRegistry.Default.GetInstance(typeof (TestClass2), KEY6);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);
            Assert.IsNotNull(instance4);
            Assert.IsNotNull(instance5);
            Assert.IsNotNull(instance6);

            var createdInstances = ServiceRegistry.Default.GetAllCreatedInstances(typeof (TestClass1));
            Assert.AreEqual(4, createdInstances.Count());

            var allInstances = ServiceRegistry.Default.GetAllInstances(typeof (TestClass1));
            Assert.AreEqual(5, allInstances.Count()); // including default instance

            foreach (var instance in allInstances)
            {
                Assert.IsNotNull(instance);

                if (instance.Equals(instance1))
                {
                    instance1 = null;
                }

                if (instance.Equals(instance2))
                {
                    instance2 = null;
                }

                if (instance.Equals(instance3))
                {
                    instance3 = null;
                }

                if (instance.Equals(instance4))
                {
                    instance4 = null;
                }

                if (instance.Equals(instance5))
                {
                    instance5 = null;
                }

                if (instance.Equals(instance6))
                {
                    instance6 = null;
                }
            }

            Assert.IsNull(instance1);
            Assert.IsNull(instance2);
            Assert.IsNull(instance3);
            Assert.IsNull(instance4);
            Assert.IsNotNull(instance5);
            Assert.IsNotNull(instance6);
        }

        [TestMethod]
        public void GetAllInstancesWithTypeGenericWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";
            const string KEY3 = "MyKey3";
            const string KEY4 = "MyKey4";
            const string KEY5 = "MyKey5";
            const string KEY6 = "MyKey6";

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();
            ServiceRegistry.Default.Register<TestClass2>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);
            var instance3 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY3);
            var instance4 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY4);
            var instance5 = ServiceRegistry.Default.GetInstance<TestClass2>(KEY5);
            var instance6 = ServiceRegistry.Default.GetInstance<TestClass2>(KEY6);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);
            Assert.IsNotNull(instance4);
            Assert.IsNotNull(instance5);
            Assert.IsNotNull(instance6);

            var createdInstances = ServiceRegistry.Default.GetAllCreatedInstances<TestClass1>();
            Assert.AreEqual(4, createdInstances.Count());

            var allInstances = ServiceRegistry.Default.GetAllInstances<TestClass1>();
            Assert.AreEqual(5, allInstances.Count()); // including default instance

            foreach (var instance in allInstances)
            {
                Assert.IsNotNull(instance);

                if (instance.Equals(instance1))
                {
                    instance1 = null;
                }

                if (instance.Equals(instance2))
                {
                    instance2 = null;
                }

                if (instance.Equals(instance3))
                {
                    instance3 = null;
                }

                if (instance.Equals(instance4))
                {
                    instance4 = null;
                }

                if (instance.Equals(instance5))
                {
                    instance5 = null;
                }

                if (instance.Equals(instance6))
                {
                    instance6 = null;
                }
            }

            Assert.IsNull(instance1);
            Assert.IsNull(instance2);
            Assert.IsNull(instance3);
            Assert.IsNull(instance4);
            Assert.IsNotNull(instance5);
            Assert.IsNotNull(instance6);
        }

        [TestMethod]
        public void GetEmptyInstancesWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "My key 1";
            const string KEY2 = "My key 2";
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();
            ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);

            // --- Act
            var allInstances = ServiceRegistry.Default.GetAllInstances(typeof (TestClass2));

            // --- Assert
            Assert.IsNotNull(allInstances);
            Assert.AreEqual(0, allInstances.Count());
        }

        [TestMethod]
        public void GetEmptyInstancesGenericWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "My key 1";
            const string KEY2 = "My key 2";
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();
            ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);

            // --- Act
            var allInstances = ServiceRegistry.Default.GetAllInstances<TestClass2>();

            // --- Assert
            Assert.IsNotNull(allInstances);
            Assert.AreEqual(0, allInstances.Count());
        }

        [TestMethod]
        public void GetInstanceWithNullKeyWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "My key 1";
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);

            // --- Act
            var instance01 = ServiceRegistry.Default.GetInstance<TestClass1>(null);
            var instance02 = ServiceRegistry.Default.GetInstance<TestClass1>(string.Empty);
            var instance03 = ServiceRegistry.Default.GetInstance<TestClass1>();

            // --- Assert
            Assert.AreNotSame(instance1, instance01);
            Assert.AreSame(instance01, instance02);
            Assert.AreSame(instance01, instance03);
            Assert.AreSame(instance02, instance03);
        }

        [TestMethod]
        public void GetInstancesWithInstanceWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";

            var instanceOriginal1 = new TestClass1();
            var instanceOriginal2 = new TestClass1();

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass>(() => instanceOriginal1, KEY1);
            ServiceRegistry.Default.Register<ITestClass>(() => instanceOriginal2, KEY2);

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY1);
            var instance3 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY2);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);

            Assert.IsInstanceOfType(instance1, typeof (TestClass1));
            Assert.IsInstanceOfType(instance2, typeof (TestClass1));
            Assert.IsInstanceOfType(instance3, typeof (TestClass1));

            Assert.AreSame(instance1, instance2);
            Assert.AreNotSame(instance1, instance3);
        }

        [TestMethod]
        public void GetInstancesWithInstanceGenericWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";

            var instanceOriginal1 = new TestClass1();
            var instanceOriginal2 = new TestClass1();

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass>(() => instanceOriginal1, KEY1);
            ServiceRegistry.Default.Register<ITestClass>(() => instanceOriginal2, KEY2);

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY1);
            var instance3 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY2);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);

            Assert.IsInstanceOfType(instance1, typeof (TestClass1));
            Assert.IsInstanceOfType(instance2, typeof (TestClass1));
            Assert.IsInstanceOfType(instance3, typeof (TestClass1));

            Assert.AreSame(instance1, instance2);
            Assert.AreNotSame(instance1, instance3);
        }

        [TestMethod]
        public void GetInstancesWithInterfaceWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY1);
            var instance3 = ServiceRegistry.Default.GetInstance(typeof (ITestClass), KEY2);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);

            Assert.IsInstanceOfType(instance1, typeof (TestClass1));
            Assert.IsInstanceOfType(instance2, typeof (TestClass1));
            Assert.IsInstanceOfType(instance3, typeof (TestClass1));

            Assert.AreSame(instance1, instance2);
            Assert.AreNotSame(instance1, instance3);
        }

        [TestMethod]
        public void GetInstancesWithInterfaceGenericWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY1);
            var instance3 = ServiceRegistry.Default.GetInstance<ITestClass>(KEY2);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);

            Assert.IsInstanceOfType(instance1, typeof (TestClass1));
            Assert.IsInstanceOfType(instance2, typeof (TestClass1));
            Assert.IsInstanceOfType(instance3, typeof (TestClass1));

            Assert.AreSame(instance1, instance2);
            Assert.AreNotSame(instance1, instance3);
        }

        [TestMethod]
        public void GetInstancesWithParametersWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "My key 1";
            const string KEY2 = "My key 2";

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();
            ServiceRegistry.Default.Register<TestClass3>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass3>(KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance<TestClass3>(KEY2);
            var property = ServiceRegistry.Default.GetInstance<ITestClass>();

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof (TestClass3));

            Assert.IsNotNull(instance2);
            Assert.IsInstanceOfType(instance2, typeof (TestClass3));

            Assert.AreNotSame(instance1, instance2);

            Assert.IsNotNull(instance1.SavedProperty);
            Assert.AreSame(instance1.SavedProperty, property);
            Assert.IsNotNull(instance2.SavedProperty);
            Assert.AreSame(instance2.SavedProperty, property);
        }

        [TestMethod]
        public void GetInstancesWithTypeWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance(typeof (TestClass1), KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance(typeof (TestClass1), KEY1);
            var instance3 = ServiceRegistry.Default.GetInstance(typeof (TestClass1), KEY2);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);

            Assert.IsInstanceOfType(instance1, typeof (TestClass1));
            Assert.IsInstanceOfType(instance2, typeof (TestClass1));
            Assert.IsInstanceOfType(instance3, typeof (TestClass1));

            Assert.AreSame(instance1, instance2);
            Assert.AreNotSame(instance1, instance3);
        }

        [TestMethod]
        public void GetInstancesWithTypeGenericWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "MyKey1";
            const string KEY2 = "MyKey2";

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var instance3 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);

            // --- Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance3);

            Assert.IsInstanceOfType(instance1, typeof (TestClass1));
            Assert.IsInstanceOfType(instance2, typeof (TestClass1));
            Assert.IsInstanceOfType(instance3, typeof (TestClass1));

            Assert.AreSame(instance1, instance2);
            Assert.AreNotSame(instance1, instance3);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OverwritingDefaultClassWithFactoryFails()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act
            ServiceRegistry.Default.Register(() => new TestClass1());
        }

        [TestMethod]
        public void OverwritingDefaultClassWithSameDefaultClassWorksAsExpected()
        {
            // --- Arrange
            const string KEY1 = "key1";
            const string KEY2 = "key2";
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act
            var defaultInstance1 = ServiceRegistry.Default.GetInstance<TestClass1>();
            var instance11 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var instance12 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);

            ServiceRegistry.Default.Register<TestClass1>();

            var defaultInstance2 = ServiceRegistry.Default.GetInstance<TestClass1>();
            var instance21 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            var instance22 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);

            // --- Assert
            Assert.AreSame(defaultInstance1, defaultInstance2);
            Assert.AreSame(instance11, instance21);
            Assert.AreSame(instance12, instance22);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OverwritingFactoryWithDefaultClassFails()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register(() => new TestClass1());
            
            // --- Act
            ServiceRegistry.Default.Register<TestClass1>();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OverwritingFactoryWithFactoryFails()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register(() => new TestClass1());

            // --- Act
            ServiceRegistry.Default.Register(() => new TestClass1());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OverwritingInterfaceClassWithOtherClassFails()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();

            // --- Act
            ServiceRegistry.Default.Register<ITestClass, TestClass4>();
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void GettingDefaultInstanceAfterRegisteringFactoryAndKeyFails()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();
            var instance = new TestClass1();
            ServiceRegistry.Default.Register(() => instance, KEY1);

            // --- Act
            ServiceRegistry.Default.GetInstance<TestClass1>();
        }

        [TestMethod]
        public void GetAllInstancesGenericWorksAsExpected1()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            const string KEY1 = "key1";
            var instance1 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance1, KEY1);
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act/Assert
            var instances = ServiceRegistry.Default.GetAllInstances<TestClass1>().ToList();
            Assert.AreEqual(2, instances.Count);

            var getInstance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            Assert.AreSame(instance1, getInstance1);

            Assert.IsTrue(instances.Contains(instance1));

            instances.Remove(instance1);
            Assert.AreEqual(1, instances.Count);

            var getInstance2 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreSame(instances[0], getInstance2);

            ServiceRegistry.Default.GetInstance<TestClass1>("key2");

            instances = ServiceRegistry.Default.GetAllInstances<TestClass1>().ToList();
            Assert.AreEqual(3, instances.Count);
        }

        [TestMethod]
        public void GetAllInstancesGenericWorksAsExpected2()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            var instance = new TestClass1();
            ServiceRegistry.Default.Register(() => instance);

            // --- Act/Assert
            var instances = ServiceRegistry.Default.GetAllInstances<TestClass1>();
            Assert.AreEqual(1, instances.Count());

            ServiceRegistry.Default.GetInstance<TestClass1>("key1");

            instances = ServiceRegistry.Default.GetAllInstances<TestClass1>();
            Assert.AreEqual(2, instances.Count());
        }

        [TestMethod]
        public void GetAllInstancesGenericWorksAsExpected3()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1(), "key1");
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1(), "key2");

            // --- Act
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>();

            // --- Assert
            Assert.AreEqual(3, instances.Count());
        }

        [TestMethod]
        public void GetAllInstancesGenericWorksAsExpected4()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1());
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1(), "key1");
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1(), "key2");

            // --- Act
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>();

            // --- Assert
            Assert.AreEqual(3, instances.Count());
        }

        [TestMethod]
        public void GetAllInstancesWorksAsExpected1()
        {
            // --- Arrange
            const string KEY1 = "key1";
            ServiceRegistry.Default.Reset();
            var instance1 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance1, KEY1);
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act/Assert
            var instances = ServiceRegistry.Default.GetAllInstances<TestClass1>().ToList();
            Assert.AreEqual(2, instances.Count);

            var getInstance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            Assert.AreSame(instance1, getInstance1);

            Assert.IsTrue(instances.Contains(instance1));

            instances.Remove(instance1);
            Assert.AreEqual(1, instances.Count);

            var getInstance2 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreSame(instances[0], getInstance2);

            ServiceRegistry.Default.GetInstance<TestClass1>("key2");

            instances = ServiceRegistry.Default.GetAllInstances<TestClass1>().ToList();
            Assert.AreEqual(3, instances.Count);
        }

        [TestMethod]
        public void GetAllInstancesWorksAsExpected2()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            var instance = new TestClass1();
            ServiceRegistry.Default.Register(() => instance);

            // --- Act/Assert
            var instances = ServiceRegistry.Default.GetAllInstances<TestClass1>();
            Assert.AreEqual(1, instances.Count());

            ServiceRegistry.Default.GetInstance<TestClass1>("key1");

            instances = ServiceRegistry.Default.GetAllInstances<TestClass1>();
            Assert.AreEqual(2, instances.Count());
        }

        [TestMethod]
        public void TestGetAllInstancesWorksAsExpected3()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1(), "key1");
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1(), "key2");

            // --- Act
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>();

            // --- Assert
            Assert.AreEqual(3, instances.Count());
        }

        [TestMethod]
        public void GetAllInstancesWorksAsExpected4()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1());
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1(), "key1");
            ServiceRegistry.Default.Register<ITestClass>(() => new TestClass1(), "key2");

            // --- Act
            var instances = ServiceRegistry.Default.GetAllInstances<ITestClass>();

            // --- Assert
            Assert.AreEqual(3, instances.Count());
        }

        [TestMethod]
        public void ConstructWithPropertyWorksAsExpected()
        {
            // --- Arrange
            var property = new TestClass1();
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register(
                () => new TestClass6
                {
                    MyProperty = property
                });

            // --- Act/Assert
            var instance1 = new TestClass6();
            Assert.IsNotNull(instance1);
            Assert.IsNull(instance1.MyProperty);

            var instance2 = ServiceRegistry.Default.GetInstance<TestClass6>();
            Assert.IsNotNull(instance2);
            Assert.IsNotNull(instance2.MyProperty);
            Assert.AreSame(property, instance2.MyProperty);
        }

        [TestMethod]
        public void DefaultClassCreationWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();
            ServiceRegistry.Default.Register<TestClass2>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>();
            var instance2 = ServiceRegistry.Default.GetInstance<TestClass2>();

            // --- Assert
            Assert.IsInstanceOfType(instance1, typeof(TestClass1));
            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance2, typeof(TestClass2));
            Assert.IsNotNull(instance2);
        }

        [TestMethod]
        public void GetInstanceWithGenericInterfaceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            var instanceOriginal = new TestClass1();
            ServiceRegistry.Default.Register<ITestClass>(() => instanceOriginal);

            // --- Act
            var instance = ServiceRegistry.Default.GetInstance<ITestClass>();

            // --- Assert
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(TestClass1));
            Assert.AreSame(instanceOriginal, instance);
        }

        [TestMethod]
        public void GetInstanceWithGenericTypeWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act
            var instance = ServiceRegistry.Default.GetInstance<TestClass1>();

            // --- Assert
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(TestClass1));
        }

        [TestMethod]
        public void GetInstanceWithInterfaceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            var instanceOriginal = new TestClass1();
            ServiceRegistry.Default.Register<ITestClass>(() => instanceOriginal);

            // --- Act
            var instance = ServiceRegistry.Default.GetInstance(typeof(ITestClass));

            // --- Assert
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(TestClass1));
            Assert.AreSame(instanceOriginal, instance);
        }

        [TestMethod]
        public void GetInstanceWithParametersWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();
            ServiceRegistry.Default.Register<TestClass3>();

            // --- Act
            var instance = ServiceRegistry.Default.GetInstance<TestClass3>();
            var property = ServiceRegistry.Default.GetInstance<ITestClass>();

            // --- Assert
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(TestClass3));

            Assert.IsNotNull(instance.SavedProperty);
            Assert.AreSame(instance.SavedProperty, property);
        }

        [TestMethod]
        public void GetInstanceWithTypeWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act
            var instance = ServiceRegistry.Default.GetInstance(typeof(TestClass1));

            // --- Assert
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(TestClass1));
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void GetInstanceWithUnregisteredClassFails()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.GetInstance<ServiceContainerTest>();
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void RegisterInstanceWithMultiConstructorsFails()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act
            ServiceRegistry.Default.Register<TestClassWithMultiConstructors>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterInterfaceOnlyFails()
        {
            ServiceRegistry.Default.Register<ITestClass>();
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void GetInstanceFailsAfterReset()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            var instanceOriginal = new TestClass1();
            ServiceRegistry.Default.Register<ITestClass>(() => instanceOriginal);
            var instance = ServiceRegistry.Default.GetInstance<ITestClass>();
            Assert.IsNotNull(instance);

            // --- Act
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.GetInstance<ITestClass>();
        }

        [TestMethod]
        public void GetDefaultWithoutCachingWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstanceWithoutCaching<TestClass1>();
            var instance2 = ServiceRegistry.Default.GetInstanceWithoutCaching<TestClass1>();

            // --- Assert
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());
            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void GetFromFactoryWithoutCachingWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register(() => new TestClass1());

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstanceWithoutCaching<TestClass1>();
            var instance2 = ServiceRegistry.Default.GetInstanceWithoutCaching<TestClass1>();

            // --- Assert
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());
            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void GetWithKeyWithoutCachingWorksAsExpected()
        {
            // --- Arrange
            const string KEY = "key1";
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act
            var instance1 = ServiceRegistry.Default.GetInstanceWithoutCaching<TestClass1>(KEY);
            var instance2 = ServiceRegistry.Default.GetInstanceWithoutCaching<TestClass1>(KEY);

            // --- Assert
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());
            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void MixCacheAndNoCacheWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act/Assert
            var instance1 = ServiceRegistry.Default.GetInstanceWithoutCaching<TestClass1>();
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsTrue(ServiceRegistry.Default.ContainsCreated<TestClass1>());
            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        [ExpectedException(typeof(ActivationException))]
        public void GetInstanceFailsAfterUnregisterClass()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.IsNotNull(instance1);
            ServiceRegistry.Default.Unregister<TestClass1>();

            // --- Act
            ServiceRegistry.Default.GetInstance<TestClass1>();
        }

        [TestMethod]
        public void GetInstanceFailsAfterUnregisterInstance()
        {
            // --- Arrange
            var instanceOriginal1 = new TestClass1();
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register(() => instanceOriginal1);

            // --- Act/Assert
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreSame(instanceOriginal1, instance1);
            ServiceRegistry.Default.Unregister(instanceOriginal1);

            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void GetInstanceWorksAfterUnregisterInstanceWithKeyAsExpected()
        {
            // --- Arrange
            const string KEY1 = "My key 1";
            const string KEY2 = "My key 2";
            var instanceOriginal1 = new TestClass1();
            var instanceOriginal2 = new TestClass1();

            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register(() => instanceOriginal1, KEY1);
            ServiceRegistry.Default.Register(() => instanceOriginal2, KEY2);

            // --- Act/Assert
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
            Assert.AreSame(instanceOriginal1, instance1);
            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY2);
            Assert.AreSame(instanceOriginal2, instance2);

            ServiceRegistry.Default.Unregister<TestClass1>(KEY1);

            try
            {
                ServiceRegistry.Default.GetInstance<TestClass1>(KEY1);
                Assert.Fail("ActivationException was expected");
            }
            catch (ActivationException)
            {
            }
        }

        [TestMethod]
        public void UnregisterAndImmediateRegisterWithInterfaceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            // --- Act/Assert
            ServiceRegistry.Default.Register<ITestClass, TestClass1>();
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<ITestClass>());

            ServiceRegistry.Default.Unregister<ITestClass>();
            Assert.IsFalse(ServiceRegistry.Default.IsRegistered<ITestClass>());

            ServiceRegistry.Default.Register<ITestClass, TestClass1>();
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<ITestClass>());
        }

        public void UnregisterInstanceAndGetNewInstanceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act/Assert
            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>();
            ServiceRegistry.Default.Unregister(instance1);
            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreNotEqual(instance1.Identifier, instance2.Identifier);
        }

        [TestMethod]
        public void UnregisterFactoryInstanceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();

            var instance0 = new TestClass1();
            ServiceRegistry.Default.Register(() => instance0);

            // --- Act/Assert
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreSame(instance0, instance1);

            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsTrue(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreSame(instance0, instance2);

            ServiceRegistry.Default.Unregister(instance0);

            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance3 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreSame(instance0, instance3);
        }

        [TestMethod]
        public void UnregisterDefaultInstanceWorksAsExpected()
        {
            // --- Arrange
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act/Assert
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>();

            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsTrue(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreSame(instance1, instance2);

            ServiceRegistry.Default.Unregister(instance1);

            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance3 = ServiceRegistry.Default.GetInstance<TestClass1>();
            Assert.AreNotSame(instance1, instance3);
        }

        [TestMethod]
        public void UnregisterKeyedInstanceWorksAsExpected()
        {
            // --- Arrange
            const string KEY = "key1";
            ServiceRegistry.Default.Reset();
            ServiceRegistry.Default.Register<TestClass1>();

            // --- Act/Assert
            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance1 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY);

            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsTrue(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance2 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY);
            Assert.AreSame(instance1, instance2);

            ServiceRegistry.Default.Unregister(instance1);

            Assert.IsTrue(ServiceRegistry.Default.IsRegistered<TestClass1>());
            Assert.IsFalse(ServiceRegistry.Default.ContainsCreated<TestClass1>());

            var instance3 = ServiceRegistry.Default.GetInstance<TestClass1>(KEY);
            Assert.AreNotSame(instance1, instance3);
        }

        #region Stubs

        public interface ITestClass
        {
        }

        public class TestClass1 : ITestClass
        {
            public static int InstancesCount { get; private set; }

            public static void Reset()
            {
                InstancesCount = 0;
            }

            public TestClass1()
            {
                Identifier = Guid.NewGuid().ToString();
                InstancesCount++;
            }

            public string Identifier { get; private set; }
        }

        public class TestClass2
        {
        }

        public class TestClass3
        {
            public ITestClass SavedProperty { get; set; }

            public TestClass3(ITestClass parameter)
            {
                SavedProperty = parameter;
            }
        }

        public class TestClass4 : ITestClass
        {
        }

        public class TestBaseClass
        {
            public void Remove()
            {
                ServiceRegistry.Default.Unregister(this);
            }
        }

        public class TestClassForCreationTime : ITestClass
        {
            public static int InstancesCreated { get; private set; }

            public TestClassForCreationTime()
            {
                InstancesCreated++;
            }

            public static void Reset()
            {
                InstancesCreated = 0;
            }
        }

        public class TestClass5
        {
            public ITestClass MyProperty { get; private set; }

            public TestClass5()
            {

            }

            [PreferredConstructor]
            public TestClass5(ITestClass myProperty)
            {
                MyProperty = myProperty;
            }
        }

        public class TestClass6
        {
            public ITestClass MyProperty { get; set; }

            public TestClass6()
            {

            }

            public TestClass6(ITestClass myProperty)
            {
                MyProperty = myProperty;
            }
        }

        public class TestClass7
        {
            private TestClass7()
            {
            }

            static TestClass7()
            {

            }
        }

        public class TestClass8
        {
            static TestClass8()
            {

            }

            public TestClass8()
            {

            }
        }

        public class TestClass9
        {
            public TestClass9()
            {

            }

            internal TestClass9(string param)
            {

            }
        }

        public class TestClassWithMultiConstructors
        {
            public TestClassWithMultiConstructors()
            {

            }

            public TestClassWithMultiConstructors(string parameter)
            {

            }
        }

        #endregion
    }
}