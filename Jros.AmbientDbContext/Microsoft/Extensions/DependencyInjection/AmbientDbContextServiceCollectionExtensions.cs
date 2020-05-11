using System;
using System.Collections.Generic;
using System.Text;
using Jros.AmbientDbContext;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AmbientDbContextServiceCollectionExtensions
    {
        public static IServiceCollection AddAmbientDbContext(this IServiceCollection sc, Action<AmbientDbContextConfiguration> configureAction = null)
        {
            sc.AddSingleton<IDbContextScopeFactory, DbContextScopeFactory>();
            sc.AddTransient<IAmbientDbContextLocator, AmbientDbContextLocator>();
            sc.AddTransient<IDbContextFactory, ServiceProviderDbContextFactory>();

            var configuration = new AmbientDbContextConfiguration(sc);

            sc.AddSingleton(configuration);

            configureAction?.Invoke(configuration);

            return sc;
        }
    }
}
