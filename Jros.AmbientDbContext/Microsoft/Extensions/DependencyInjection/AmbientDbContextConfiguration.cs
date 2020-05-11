namespace Microsoft.Extensions.DependencyInjection
{
    public class AmbientDbContextConfiguration
    {
        private readonly IServiceCollection _sc;

        public AmbientDbContextConfiguration(IServiceCollection sc)
        {
            _sc = sc;
        }

        //public AmbientDbContextConfiguration AddDbContext<TDbContext() 
    }
}
