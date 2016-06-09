using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;


namespace Hangfire.Unity.Test
{
    [TestClass]
    public class UnityJobActivatorTests
    {
        private IUnityContainer container;

        public UnityJobActivatorTests()
        {
            container = new UnityContainer();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ThrowsAnException_WhenLifetimeScopeIsNull()
        {
            // ReSharper disable once UnusedVariable
            var activator = new UnityJobActivator(null);
        }

        [TestMethod]
        public void Class_IsBasedOnJobActivator()
        {
            var activator = CreateActivator();
            Assert.IsInstanceOfType(activator, typeof(UnityJobActivator));
        }

        [TestMethod]
        public void InstancePerContainer_RegistersSameServiceInstance_ForDifferentScopeInstances()
        {
            container.RegisterType<object>(
           new ContainerControlledLifetimeManager(),
           new InjectionFactory(c => new object()));

            var activator = CreateActivator();

            object instance1;
            using (var scope1 = activator.BeginScope())
            {
                instance1 = scope1.Resolve(typeof(object));
            }

            object instance2;
            using (var scope2 = activator.BeginScope())
            {
                instance2 = scope2.Resolve(typeof(object));
            }

            Assert.AreSame(instance1, instance2);
        }



        [TestMethod]
        public void InstancePerBackgroundJob_RegistersDifferentServiceInstances_ForDifferentScopeInstances()
        {
            container.RegisterType<object>(
           new HierarchicalLifetimeManager(),
           new InjectionFactory(c => new object()));

            var activator = CreateActivator();

            object instance1;
            using (var scope1 = activator.BeginScope())
            {
                instance1 = scope1.Resolve(typeof(object));
            }

            object instance2;
            using (var scope2 = activator.BeginScope())
            {
                instance2 = scope2.Resolve(typeof(object));
            }

            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void InstancePerJob_RegistersSameServiceInstance_ForTheSameScopeInstance()
        {
            container.RegisterType<object>(
                     new HierarchicalLifetimeManager(),
                     new InjectionFactory(c => new object()));

            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance1 = scope.Resolve(typeof(object));
                var instance2 = scope.Resolve(typeof(object));

                Assert.AreSame(instance1, instance2);
            }
        }

        [TestMethod]
        public void InstancePerJob_RegistersDifferentServiceInstances_ForDifferentScopeInstances()
        {
            container.RegisterType<object>(
                     new HierarchicalLifetimeManager(),
                     new InjectionFactory(c => new object()));

            var activator = CreateActivator();

            object instance1;
            using (var scope1 = activator.BeginScope())
            {
                instance1 = scope1.Resolve(typeof(object));
            }

            object instance2;
            using (var scope2 = activator.BeginScope())
            {
                instance2 = scope2.Resolve(typeof(object));
            }

            Assert.AreNotSame(instance1, instance2);
        }


        private UnityJobActivator CreateActivator()
        {
            return new UnityJobActivator(container);
        }

        class Disposable : IDisposable
        {
            public bool Disposed { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }


    }
}
