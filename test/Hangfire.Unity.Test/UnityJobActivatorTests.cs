using System;
using System.Collections.Generic;
using Hangfire.Annotations;
using Hangfire.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

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
            using (var scope1 = new  SimpleJobActivatorScope(activator))
            {
                instance1 = scope1.Resolve(typeof(object));
            }

            object instance2;
            using (var scope2 = new  SimpleJobActivatorScope(activator))
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
            using (var scope1 = new  SimpleJobActivatorScope(activator))
            {
                instance1 = scope1.Resolve(typeof(object));
            }

            object instance2;
            using (var scope2 = new  SimpleJobActivatorScope(activator))
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

            using (var scope = new  SimpleJobActivatorScope(activator))
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
            using (var scope1 =new  SimpleJobActivatorScope(activator))
            {
                instance1 = scope1.Resolve(typeof(object));
            }

            object instance2;
            using (var scope2 = new  SimpleJobActivatorScope(activator))
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
        class SimpleJobActivatorScope : JobActivatorScope
        {
            private readonly JobActivator _activator;
            private readonly List<IDisposable> _disposables = new List<IDisposable>();

            public SimpleJobActivatorScope([NotNull] JobActivator activator)
            {
                if (activator == null) throw new ArgumentNullException(nameof(activator));
                _activator = activator;
            }

            public override object Resolve(Type type)
            {
                var instance = _activator.ActivateJob(type);
                var disposable = instance as IDisposable;

                if (disposable != null)
                {
                    _disposables.Add(disposable);
                }

                return instance;
            }

            public override void DisposeScope()
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }
        }

    }
}
