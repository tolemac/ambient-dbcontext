using System.Linq;
using Jros.AmbientDbContext.Collections;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Jros.AmbientDbContext.Tests
{
    public class DbContextCollection_Tests
    {
        [Fact]
        public void Should_Instantiate_One_DbContext_Using_Several_Interfaces()
        {
            var factory = new DbContextFactory();
            var collection = new DbContextCollection(false, null, factory);

            var dbContext1 = collection.Get<TestDbContext>();
            var dbContext2 = collection.Get<ITestDbContext1>();
            var dbContext3 = collection.Get<ITestDbContext2>();

            Assert.Same(dbContext1, dbContext2);
            Assert.Same(dbContext1, dbContext3);
            Assert.True(collection.InitializedDbContexts.GetDbContexts().ToArray().Length == 1);

        }

        public class DbContextFactory : IDbContextFactory
        {
            public TDbContext CreateDbContext<TDbContext>() where TDbContext : class
            {
                return new TestDbContext() as TDbContext;
            }
        }

        public interface ITestDbContext1 { }
        public interface ITestDbContext2 { }

        public class TestDbContext : DbContext, ITestDbContext1, ITestDbContext2 { }
    }
}
