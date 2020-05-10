using Jros.AmbientDbContext.Tests.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Jros.AmbientDbContext.Tests.DatabaseContext
{
    public interface IUserManagementDbContext
    {
        DbSet<User> Users { get; }
    }
}
