using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeemplestLight.Core.Portable.DependencyInjection;
using Shouldly;

namespace SeemplestLight.Core.Test.DependencyInjection
{
    [TestClass]
    public class ServiceRegistryTest
    {
        [TestMethod]
        public void ConstructionWorks()
        {
            // --- Act
            var sr = new ServiceRegistry();

            // --- Arrange
            sr.Parent.ShouldBeNull();
        }

        [TestMethod]
        public void ConstructionWorksWithParent()
        {
            // --- Arrange
            var parentSr = new ServiceRegistry();

            // --- Act
            var sr = new ServiceRegistry(parentSr);

            // --- Arrange
            sr.Parent.ShouldBe(parentSr);
        }

        [TestMethod]
        public void GetServiceWorks()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());

            // --- Act
            var service = sr.GetService<IMyService>();

            // --- Assert
            service.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetServiceWithRegisteredNamedInstanceWorks()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());
            sr.Register<IMyService>(() => new MyService(), "namedInstance");
            var service = sr.GetService<IMyService>();

            // --- Act
            var namedService = sr.GetService<IMyService>("namedInstance");

            // --- Assert
            namedService.ShouldNotBeNull();
            namedService.ShouldNotBe(service);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void GetServiceWithUnknownServiceRaisesException()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());

            // --- Act
            sr.GetService<int>();
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void GetServiceWithUnknownNamedInstanceRaisesException1()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());
            sr.Register<IMyService>(() => new MyService(), "namedInstance");
            try
            {
                sr.GetService<IMyService>();
            }
            catch (ServiceNotFoundException)
            {
                Assert.Fail("Getting default instance should work");
            }

            // --- Act
            sr.GetService<IMyService>("otherInstance");
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void GetServiceWithUnknownNamedInstanceRaisesException2()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());
            try
            {
                sr.GetService<IMyService>();
            }
            catch (ServiceNotFoundException)
            {
                Assert.Fail("Getting default instance should work");
            }

            // --- Act
            sr.GetService<IMyService>("namedInstance");
        }

        [TestMethod]
        public void NonGenericGetServiceWorks()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());

            // --- Act
            var service = sr.GetService(typeof(IMyService));

            // --- Assert
            service.ShouldNotBeNull();
        }

        [TestMethod]
        public void NonGenericGetServiceWithRegisteredNamedInstanceWorks()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());
            sr.Register<IMyService>(() => new MyService(), "namedInstance");
            var service = sr.GetService<IMyService>();

            // --- Act
            var namedService = sr.GetService(typeof(IMyService), "namedInstance");

            // --- Assert
            namedService.ShouldNotBeNull();
            namedService.ShouldNotBe(service);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void NonGenericGetServiceWithUnknownServiceRaisesException()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());

            // --- Act
            sr.GetService(typeof(int));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void NonGenericGetServiceWithUnknownNamedInstanceRaisesException1()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());
            sr.Register<IMyService>(() => new MyService(), "namedInstance");
            try
            {
                sr.GetService(typeof(IMyService));
            }
            catch (ServiceNotFoundException)
            {
                Assert.Fail("Getting default instance should work");
            }

            // --- Act
            sr.GetService(typeof(IMyService), "otherInstance");
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceNotFoundException))]
        public void NonGenericGetServiceWithUnknownNamedInstanceRaisesException2()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());
            try
            {
                sr.GetService(typeof(IMyService));
            }
            catch (ServiceNotFoundException)
            {
                Assert.Fail("Getting default instance should work");
            }

            // --- Act
            sr.GetService(typeof(IMyService), "namedInstance");
        }

        [TestMethod]
        public void GetServiceInvokesFactoryEveryTime()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());
            MyService.InstanceCount = 0;

            // --- Act
            var service1 = sr.GetService<IMyService>();
            service1.DoSomething();
            var count1 = MyService.InstanceCount;
            var service2 = sr.GetService<IMyService>();
            service2.DoSomething();
            var count2 = MyService.InstanceCount;

            // --- Assert
            service1.ShouldNotBe(service2);
            count1.ShouldBe(1);
            count2.ShouldBe(2);
        }

        [TestMethod]
        public void GetServiceWorksWithCachingFactory()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            var cachedInstance = new MyService();
            sr.Register<IMyService>(() => cachedInstance);
            MyService.InstanceCount = 0;

            // --- Act
            var service1 = sr.GetService<IMyService>();
            service1.DoSomething();
            var count1 = MyService.InstanceCount;
            var service2 = sr.GetService<IMyService>();
            service2.DoSomething();
            var count2 = MyService.InstanceCount;

            // --- Assert
            service1.ShouldBe(service2);
            count1.ShouldBe(1);
            count2.ShouldBe(2);
        }

        [TestMethod]
        public void GetServiceWithNamedInstancesInvokesFactoryEveryTime()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService(), "one");
            sr.Register<IMyService>(() => new MyService(), "two");
            MyService.InstanceCount = 0;

            // --- Act
            var service1 = sr.GetService<IMyService>("one");
            service1.DoSomething();
            var count1 = MyService.InstanceCount;
            var service2 = sr.GetService<IMyService>("two");
            service2.DoSomething();
            var count2 = MyService.InstanceCount;

            // --- Assert
            service1.ShouldNotBe(service2);
            count1.ShouldBe(1);
            count2.ShouldBe(2);
        }

        [TestMethod]
        public void ResetRemovesServiceRegistrations()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());
            var isRegistered = sr.IsRegistered<IMyService>();

            // --- Act
            sr.Reset();

            // --- Assert
            isRegistered.ShouldBeTrue();
            sr.IsRegistered<IMyService>().ShouldBeFalse();
        }

        [TestMethod]
        public void ResetRemovesServicesWithNamedInstances()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService(), "one");
            sr.Register<IMyService>(() => new MyService(), "two");
            var isRegistered1 = sr.IsRegistered<IMyService>("one");
            var isRegistered2 = sr.IsRegistered<IMyService>("two");

            // --- Act
            sr.Reset();

            // --- Assert
            isRegistered1.ShouldBeTrue();
            sr.IsRegistered<IMyService>("one").ShouldBeFalse();
            isRegistered2.ShouldBeTrue();
            sr.IsRegistered<IMyService>("two").ShouldBeFalse();
        }

        [TestMethod]
        public void IsRegisteredWorks()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());

            // --- Act
            var isRegistered = sr.IsRegistered<IMyService>();
            var isRegisteredInt = sr.IsRegistered<int>();

            // --- Assert
            isRegistered.ShouldBeTrue();
            isRegisteredInt.ShouldBeFalse();
        }

        [TestMethod]
        public void IsRegisteredWorksWithNamedInstance()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService(), "one");

            // --- Act
            var isRegistered = sr.IsRegistered<IMyService>("one");
            var isRegisteredTwo = sr.IsRegistered<IMyService>("two");
            var isRegisteredInt = sr.IsRegistered<int>("any");

            // --- Assert
            isRegistered.ShouldBeTrue();
            isRegisteredTwo.ShouldBeFalse();
            isRegisteredInt.ShouldBeFalse();
        }

        [TestMethod]
        public void NonGenericIsRegisteredWorks()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());

            // --- Act
            var isRegistered = sr.IsRegistered(typeof(IMyService));
            var isRegisteredInt = sr.IsRegistered(typeof(int));

            // --- Assert
            isRegistered.ShouldBeTrue();
            isRegisteredInt.ShouldBeFalse();
        }

        [TestMethod]
        public void NonGenericIsRegisteredWorksWithNamedInstance()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService(), "one");

            // --- Act
            var isRegistered = sr.IsRegistered(typeof(IMyService), "one");
            var isRegisteredTwo = sr.IsRegistered(typeof(IMyService), "two");
            var isRegisteredInt = sr.IsRegistered(typeof(int), "any");

            // --- Assert
            isRegistered.ShouldBeTrue();
            isRegisteredTwo.ShouldBeFalse();
            isRegisteredInt.ShouldBeFalse();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterRaisesExceptionWithNullFactory()
        {
            // --- Arrange
            var sr = new ServiceRegistry();

            // --- Act
            sr.Register<IMyService>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterNamedInstanceRaisesExceptionWithNullFactory()
        {
            // --- Arrange
            var sr = new ServiceRegistry();

            // --- Act
            sr.Register<IMyService>(null, "one");
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAlreadyRegisteredException))]
        public void RegisterRaisesExceptionWithAlreadyRegisteredInstance()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());

            // --- Act
            sr.Register<IMyService>(() => new MyService());
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAlreadyRegisteredException))]
        public void RegisterNamedInstanceRaisesExceptionWithAlreadyRegisteredInstance()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService(), "one");

            // --- Act
            sr.Register<IMyService>(() => new MyService(), "one");
        }

        [TestMethod]
        public void UnregisterRemovesRegisteredInstance()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService());
            var isRegistered = sr.IsRegistered<IMyService>();

            // --- Act
            sr.Unregister<IMyService>();

            // --- Assert
            isRegistered.ShouldBeTrue();
            sr.IsRegistered<IMyService>().ShouldBeFalse();
        }

        [TestMethod]
        public void UnregisterRemovesRegisteredNamedInstance()
        {
            // --- Arrange
            var sr = new ServiceRegistry();
            sr.Register<IMyService>(() => new MyService(), "one");
            sr.Register<IMyService>(() => new MyService(), "two");
            var isRegistered = sr.IsRegistered<IMyService>("one");
            var isRegisteredTwo = sr.IsRegistered<IMyService>("two");

            // --- Act
            sr.Unregister<IMyService>("one");

            // --- Assert
            isRegistered.ShouldBeTrue();
            isRegisteredTwo.ShouldBeTrue();
            sr.IsRegistered<IMyService>("one").ShouldBeFalse();
            sr.IsRegistered<IMyService>("two").ShouldBeTrue();
        }

        [TestMethod]
        public void GetServiceWorksWithParent()
        {
            // --- Arrange
            var srParent = new ServiceRegistry();
            srParent.Register<IMyService>(() => new MyService());
            var sr = new ServiceRegistry(srParent);

            // --- Act
            var service = sr.GetService<IMyService>();

            // --- Assert
            service.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetServiceWorksWithParentChain()
        {
            // --- Arrange
            var srGrandParent = new ServiceRegistry();
            srGrandParent.Register<IMyService>(() => new MyService());
            var srParent = new ServiceRegistry(srGrandParent);
            var sr = new ServiceRegistry(srParent);

            // --- Act
            var service = sr.GetService<IMyService>();

            // --- Assert
            service.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetServiceWithNamedInstanceWorksWithParent()
        {
            // --- Arrange
            var srParent = new ServiceRegistry();
            srParent.Register<IMyService>(() => new MyService(), "one");
            var sr = new ServiceRegistry(srParent);

            // --- Act
            var service = sr.GetService<IMyService>("one");

            // --- Assert
            service.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetServiceWithNamedInstanceWorksWithParentChain()
        {
            // --- Arrange
            var srGrandParent = new ServiceRegistry();
            srGrandParent.Register<IMyService>(() => new MyService(), "one");
            var srParent = new ServiceRegistry(srGrandParent);
            var sr = new ServiceRegistry(srParent);

            // --- Act
            var service = sr.GetService<IMyService>("one");

            // --- Assert
            service.ShouldNotBeNull();
        }

        private interface IMyService
        {
            void DoSomething();
        }

        private class MyService : IMyService
        {
            public static int InstanceCount { get; set; }

            public void DoSomething()
            {
                InstanceCount++;
            }
        }
    }
}