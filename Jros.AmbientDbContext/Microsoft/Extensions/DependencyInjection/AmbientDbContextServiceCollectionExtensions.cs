using Jros.AmbientDbContext;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AmbientDbContextServiceCollectionExtensions
    {
        public static IServiceCollection AddAmbientDbContext(this IServiceCollection sc)
        {
            sc.AddSingleton<IDbContextScopeFactory, DbContextScopeFactory>();
            sc.AddTransient<IAmbientDbContextLocator, AmbientDbContextLocator>();
            sc.AddTransient<IDbContextFactory, ServiceProviderDbContextFactory>();

            return sc;
        }
    }
}
