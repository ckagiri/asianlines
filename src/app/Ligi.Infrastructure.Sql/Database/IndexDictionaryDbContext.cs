using System;
using System.Data.Entity;
using System.Linq;
using Ligi.Core.Model;

namespace Ligi.Infrastructure.Sql.Database
{
    public class IndexDictionaryDbContext : DbContext
    {
        //public const string SchemaName = "IndexDictionaries";

        public IndexDictionaryDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FixtureBookieIndex>().ToTable("FixtureBookies");
        }

        public T Find<T>(Guid id) where T : class
        {
            return Set<T>().Find(id);
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return Set<T>();
        }
    }
}
