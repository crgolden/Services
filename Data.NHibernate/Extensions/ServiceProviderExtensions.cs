namespace System
{
    using Collections.Generic;
    using Common;
    using JetBrains.Annotations;
    using Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using NHibernate;
    using Services;
    using static StringComparison;

    /// <summary>A class with methods that extend <see cref="IServiceProvider"/>.</summary>
    [PublicAPI]
    public static class ServiceProviderExtensions
    {
        internal static IDataService GetNHibernateDataService(this IServiceProvider provider, string name)
        {
            var factories = provider.GetRequiredService<IDictionary<string, ISessionFactory>>();
            var factory = factories[name];
            var session = factory.WithOptions().OpenSession();
            var monitor = provider.GetRequiredService<IOptionsMonitor<NHibernateDataOptions>>();
            var options = monitor.Get(name);
            return new NHibernateDataService(session, options, name);
        }

        internal static T GetNHibernateDataService<T>(this IServiceProvider provider, string name)
        {
            var dataServices = provider.GetServices<IDataService>();
            var dataService = dataServices.Single(x => string.Equals(x.Name, name, InvariantCultureIgnoreCase));
            return (T)dataService;
        }
    }
}
