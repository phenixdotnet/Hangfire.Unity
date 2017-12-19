using System;
using Hangfire.Annotations;
using Unity;

namespace Hangfire
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration<UnityJobActivator> UseUnityActivator(
            [NotNull] this IGlobalConfiguration configuration, IUnityContainer container)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (container == null) throw new ArgumentNullException("container");

            return configuration.UseActivator(new UnityJobActivator(container));
        }
    }
}