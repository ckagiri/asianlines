using System;
using System.Data.Entity;
using System.Linq;
using Ligi.Core.DataAccess;
using Ligi.Infrastructure.Sql.ReadModel;

namespace Ligi.Infrastructure.Sql.Database
{
    public class BetsDbContext : DbContext, IContext
    {
        public BetsDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString) 
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WeekAccount>().ToTable("WeekAccountsView");
            modelBuilder.Entity<Bet>().ToTable("BetsView");
            modelBuilder.Entity<MonthLeaderBoard>().ToTable("MonthLeaderBoardView");
            modelBuilder.Entity<SeasonLeaderBoard>().ToTable("SeasonLeaderBoardView");
        }

        public T Find<T>(Guid id) where T : class
        {
            return Set<T>().Find(id);
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return Set<T>();
        }

        public void Save<T>(T entity) where T : class
        {
            var entry = Entry(entity);

            if (entry.State == EntityState.Detached)
                Set<T>().Add(entity);

           SaveChanges();
        }
    }
}
