using Jros.AmbientDbContext.Tests.DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jros.AmbientDbContext.Tests.DatabaseContext
{
	/// <summary>
	/// Defines the convention-based mapping overrides for the User model. 
	/// </summary>
	public class UserFluentMap : IEntityTypeConfiguration<User>
	{
        public void Configure(EntityTypeBuilder<User> builder)
        {
			builder.Property(m => m.Name).IsRequired();
            builder.Property(m => m.Email).IsRequired();
        }
	}
}
