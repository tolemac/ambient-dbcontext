using System;
using Jros.AmbientDbContext;

namespace Microsoft.Extensions.DependencyInjection
{
    internal class ServiceProviderDbContextFactory : IDbContextFactory
    {
        private readonly IServiceProvider _sp;

        public ServiceProviderDbContextFactory(IServiceProvider sp)
        {
            _sp = sp;
        }
        public TDbContext CreateDbContext<TDbContext>() where TDbContext : class
        {
            return _sp.GetService<TDbContext>();
        }
    }
}
