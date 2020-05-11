using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Jros.AmbientDbContext.Collections
{
    internal class InitializedDbContextCollection : List<InitializedDbContext>
    {
        public DbContext this[Type type]
        {
            get
            {
                return this.FirstOrDefault(i => type.IsInstanceOfType(i.DbContext))?.DbContext;
            }
        }

        public IEnumerable<DbContext> GetDbContexts()
        {
            return this.Select(item => item.DbContext);
        }
    }
}
