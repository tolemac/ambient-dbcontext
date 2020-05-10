using System.Reflection;
using Jros.AmbientDbContext.Tests.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Jros.AmbientDbContext.Tests.DatabaseContext
{
	public class UserManagementDbContext : DbContext
	{
		// Map our 'User' model by convention
		public DbSet<User> Users { get; set; }

		public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
		{}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Overrides for the convention-based mappings.
			// We're assuming that all our fluent mappings are declared in this assembly.
			modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(UserManagementDbContext)));
		}
	}
}
