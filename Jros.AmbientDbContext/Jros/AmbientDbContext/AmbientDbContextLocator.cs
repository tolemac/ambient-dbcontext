/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */

namespace Jros.AmbientDbContext
{
    public class AmbientDbContextLocator : IAmbientDbContextLocator
    {
        public TDbContext Get<TDbContext>() where TDbContext : class
        {
            var ambientDbContextScope = DbContextScope.GetAmbientScope();
            return ambientDbContextScope?.DbContexts.Get<TDbContext>();
        }
    }
}