using System;
using System.Threading.Tasks;
using Jros.AmbientDbContext.Tests.DomainModel;

namespace Jros.AmbientDbContext.Tests.Repositories
{
	public interface IUserRepository 
	{
		User Get(Guid userId);
		Task<User> GetAsync(Guid userId);
		void Add(User user);
	}
}