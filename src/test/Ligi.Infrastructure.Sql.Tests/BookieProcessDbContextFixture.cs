using Ligi.Core.Processes;
using System;
using System.Data.Entity.Infrastructure;
using Ligi.Infrastructure.Sql.Processes;
using Xunit;

namespace Ligi.Infrastructure.Sql.Tests
{
    public class BookieProcessDbContextFixture
    {
        [Fact]
        public void when_saving_process_then_can_retrieve_it()
        {
            var dbName = GetType().Name + "-" + Guid.NewGuid();
            using (var context = new BettingProcessDbContext(dbName))
            {
                context.Database.Create();
            }

            try
            {
                Guid id = Guid.Empty;
                using (var context = new BettingProcessDbContext(dbName))
                {
                    var bp = new BookieProcess();
                    context.BookieProcesses.Add(bp);
                    context.SaveChanges();
                    id = bp.Id;
                }
                using (var context = new BettingProcessDbContext(dbName))
                {
                    var bp = context.BookieProcesses.Find(id);
                    Assert.NotNull(bp);
                }
            }
            finally
            {
                //using (var context = new BettingProcessDbContext(dbName))
                //{
                //    context.Database.Delete();
                //}
            }
        }

        [Fact]
        public void when_saving_process_performs_optimistic_locking()
        {
            var dbName = this.GetType().Name + "-" + Guid.NewGuid();
            using (var context = new BettingProcessDbContext(dbName))
            {
                context.Database.Create();
            }

            try
            {
                Guid id = Guid.Empty;
                using (var context = new BettingProcessDbContext(dbName))
                {
                    var bp = new BookieProcess();
                    context.BookieProcesses.Add(bp);
                    context.SaveChanges();
                    id = bp.Id;
                }

                using (var context = new BettingProcessDbContext(dbName))
                {
                    var pm = context.BookieProcesses.Find(id);

                    using (var innerContext = new BettingProcessDbContext(dbName))
                    {
                        var innerProcess = innerContext.BookieProcesses.Find(id);

                        innerContext.SaveChanges();
                    }

                    Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                }
            }
            finally
            {
                using (var context = new BettingProcessDbContext(dbName))
                {
                    context.Database.Delete();
                }
            }
        }
    }
}
