using System;
using System.Data.Entity.Infrastructure;
using Ligi.Core.Processes;
using Ligi.Infrastructure.Sql.Processes;
using Xunit;

namespace Ligi.Infrastructure.Sql.Tests
{
    public class BetProcessDbContextFixture
    {
        readonly Guid _fixtureId = Guid.NewGuid();

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
                    var bp = new BetProcess(_fixtureId);
                    context.BetProcesses.Add(bp);
                    context.SaveChanges();
                    id = bp.Id;
                }
                using (var context = new BettingProcessDbContext(dbName))
                {
                    var bp = context.BetProcesses.Find(id);
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
                    var bp = new BetProcess(_fixtureId);
                    context.BetProcesses.Add(bp);
                    context.SaveChanges();
                    id = bp.Id;
                }

                using (var context = new BettingProcessDbContext(dbName))
                {
                    var bp = context.BetProcesses.Find(id);

                    using (var innerContext = new BettingProcessDbContext(dbName))
                    {
                        var innerProcess = innerContext.BetProcesses.Find(id);

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
