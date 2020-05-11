using System;
using Microsoft.EntityFrameworkCore;

namespace Jros.AmbientDbContext.Collections
{
    internal class InitializedDbContext
    {
        public Type RequestedType { get; }
        public DbContext DbContext { get; }

        public InitializedDbContext(Type requestedType, DbContext dbContext)
        {
            RequestedType = requestedType;
            DbContext = dbContext;
        }
    }
}
