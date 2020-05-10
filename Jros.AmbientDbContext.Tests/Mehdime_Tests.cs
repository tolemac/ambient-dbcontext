using System;
using System.Linq;
using Jros.AmbientDbContext.Implementations;
using Jros.AmbientDbContext.Interfaces;
using Jros.AmbientDbContext.Tests.BusinessLogicServices;
using Jros.AmbientDbContext.Tests.CommandModel;
using Jros.AmbientDbContext.Tests.DatabaseContext;
using Jros.AmbientDbContext.Tests.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Jros.AmbientDbContext.Tests
{
    public class Mehdime_Tests
    {
        private readonly ITestOutputHelper _console;

        public Mehdime_Tests(ITestOutputHelper output)
        {
            this._console = output;
        }

		public class DbContextFactory : IDbContextFactory
        {
            private readonly SqliteConnection _conn;

            public DbContextFactory(SqliteConnection conn)
            {
                _conn = conn;
            }
            public TDbContext CreateDbContext<TDbContext>() where TDbContext : class
            {
                var options = new DbContextOptionsBuilder<UserManagementDbContext>()
                    .UseSqlite(_conn) // Set the connection explicitly, so it won't be closed automatically by EF
                    .Options;
                
                return new UserManagementDbContext(options) as TDbContext;
            }
        }

        [Fact]
        public void Mehdime_Demo()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");

            connection.Open();

            var dbContextFactory = new DbContextFactory(connection);
            using (var context = dbContextFactory.CreateDbContext<UserManagementDbContext>())
            {
                context.Database.EnsureCreated();
            } 

            var dbContextScopeFactory = new DbContextScopeFactory(dbContextFactory);
            var ambientDbContextLocator = new AmbientDbContextLocator();
            var userRepository = new UserRepository(ambientDbContextLocator);

            var userCreationService = new UserCreationService(dbContextScopeFactory, userRepository);
            var userQueryService = new UserQueryService(dbContextScopeFactory, userRepository);
            var userEmailService = new UserEmailService(dbContextScopeFactory);
            var userCreditScoreService = new UserCreditScoreService(dbContextScopeFactory);

            try
            {
                _console.WriteLine("This test is a Mehdime original demo port to XUnit test\n");
                //_console.WriteLine("This demo application will create a database named DbContextScopeDemo in the default SQL Server instance on localhost. Edit the connection string in UserManagementDbContext if you'd like to create it somewhere else.");
                //_console.WriteLine("Press enter to start...");
                //_console.ReadLine();

                //-- Demo of typical usage for read and writes
                _console.WriteLine("Creating a user called Mary...");
                var marysSpec = new UserCreationSpec("Mary", "mary@example.com");
                userCreationService.CreateUser(marysSpec);
                _console.WriteLine("Done.\n");

                _console.WriteLine("Trying to retrieve our newly created user from the data store...");
                var mary = userQueryService.GetUser(marysSpec.Id);
                _console.WriteLine("OK. Persisted user: {0}\n", mary);

                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();

                //-- Demo of nested DbContextScopes
                _console.WriteLine("Creating 2 new users called John and Jeanne in an atomic transaction...");
                var johnSpec = new UserCreationSpec("John", "john@example.com");
                var jeanneSpec = new UserCreationSpec("Jeanne", "jeanne@example.com");
                userCreationService.CreateListOfUsers(johnSpec, jeanneSpec);
                _console.WriteLine("Done.\n");

                _console.WriteLine("Trying to retrieve our newly created users from the data store...");
                var createdUsers = userQueryService.GetUsers(johnSpec.Id, jeanneSpec.Id);
                _console.WriteLine("OK. Found {0} persisted users.\n", createdUsers.Count());

                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();

                //-- Demo of nested DbContextScopes in the face of an exception. 
                // If any of the provided users failed to get persisted, none should get persisted. 
                _console.WriteLine("Creating 2 new users called Julie and Marc in an atomic transaction. Will make the persistence of the second user fail intentionally in order to test the atomicity of the transaction...");
                var julieSpec = new UserCreationSpec("Julie", "julie@example.com");
                var marcSpec = new UserCreationSpec("Marc", "marc@example.com");
                try
                {
                    userCreationService.CreateListOfUsersWithIntentionalFailure(julieSpec, marcSpec);
                    _console.WriteLine("Done.\n");
                }
                catch (Exception e)
                {
                    _console.WriteLine(e.Message);
                    _console.WriteLine("");
                }

                _console.WriteLine("Trying to retrieve our newly created users from the data store...");
                var maybeCreatedUsers = userQueryService.GetUsers(julieSpec.Id, marcSpec.Id);
                _console.WriteLine("Found {0} persisted users. If this number is 0, we're all good. If this number is not 0, we have a big problem.\n", maybeCreatedUsers.Count());

                //_console.WriteLine("Press enter to continue...");
                //Console.ReadLine();

                //-- Demo of DbContextScope within an async flow
                _console.WriteLine("Trying to retrieve two users John and Jeanne sequentially in an asynchronous manner...");
                // We're going to block on the async task here as we don't have a choice. No risk of deadlocking in any case as console apps
                // don't have a synchronization context.
                var usersFoundAsync = userQueryService.GetTwoUsersAsync(johnSpec.Id, jeanneSpec.Id).Result;
                _console.WriteLine("OK. Found {0} persisted users.\n", usersFoundAsync.Count());

                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();

                //-- Demo of explicit database transaction. 
                _console.WriteLine("Trying to retrieve user John within a READ UNCOMMITTED database transaction...");
                // You'll want to use SQL Profiler or Entity Framework Profiler to verify that the correct transaction isolation
                // level is being used.
                var userMaybeUncommitted = userQueryService.GetUserUncommitted(johnSpec.Id);
                _console.WriteLine("OK. User found: {0}\n", userMaybeUncommitted);

                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();

                //-- Demo of disabling the DbContextScope nesting behaviour in order to force the persistence of changes made to entities
                // This is a pretty advanced feature that you can safely ignore until you actually need it.
                _console.WriteLine("Will simulate sending a Welcome email to John...");

                using (var parentScope = dbContextScopeFactory.Create())
                {
                    var parentDbContext = parentScope.DbContexts.Get<IUserManagementDbContext>();

                    // Load John in the parent DbContext
                    var john = parentDbContext.Users.Find(johnSpec.Id);
                    _console.WriteLine("Before calling SendWelcomeEmail(), john.WelcomeEmailSent = " + john.WelcomeEmailSent);

                    // Now call our SendWelcomeEmail() business logic service method, which will
                    // update John in a non-nested child context
                    userEmailService.SendWelcomeEmail(johnSpec.Id);

                    // Verify that we can see the modifications made to John by the SendWelcomeEmail() method
                    _console.WriteLine("After calling SendWelcomeEmail(), john.WelcomeEmailSent = " + john.WelcomeEmailSent);

                    // Note that even though we're not calling SaveChanges() in the parent scope here, the changes
                    // made to John by SendWelcomeEmail() will remain persisted in the database as SendWelcomeEmail()
                    // forced the creation of a new DbContextScope.
                }

                _console.WriteLine("");
                //Console.WriteLine("Press enter to continue...");
                //Console.ReadLine();

                //-- Demonstration of DbContextScope and parallel programming
                _console.WriteLine("Calculating and storing the credit score of all users in the database in parallel...");
                userCreditScoreService.UpdateCreditScoreForAllUsers();
                _console.WriteLine("Done.");
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                throw;
            }

            _console.WriteLine("");
            _console.WriteLine("The end.");
            //Console.WriteLine("Press enter to exit...");
            //Console.ReadLine();
        }
    }
}
