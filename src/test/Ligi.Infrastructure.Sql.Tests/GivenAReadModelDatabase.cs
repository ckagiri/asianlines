using System;
using Ligi.Infrastructure.Sql.Database;

namespace Ligi.Infrastructure.Sql.Tests
{
    public class given_a_read_model_database : IDisposable
    {
        protected string dbName;

        public given_a_read_model_database()
        {
            dbName = GetType().Name; 
               // + "-" + Guid.NewGuid().ToString();
            using (var context = new BetsDbContext(dbName))
            {
                if (context.Database.Exists())
                    context.Database.Delete();

                context.Database.Create();
            }
        }

        public void Dispose()
        {
            using (var context = new BetsDbContext(dbName))
            {
                if (context.Database.Exists()) context.Database.Delete();
            }
        }
    }
}
