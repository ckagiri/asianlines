using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ligi.Core.Events.Domain;
using Ligi.Core.Model;
using Ligi.Core.Processes;
using Ligi.Core.Utils;
using Xunit;

namespace Ligi.Core.Tests
{
    public class BookieProcessRouterFixture
    {
        private static readonly DateTime Now = DateTime.Now;
        private static Bet _bet1 = new Bet
        {
            FixtureId = Guid.NewGuid(),
            BetType = BetType.AsianHandicap,
            BetPick = BetPick.Away,
            Handicap = 1.5m,
            Wager = 200,
            TimeStamp = Now
        };

        private static Bet _bet2 = new Bet
        {
            FixtureId = Guid.NewGuid(),
            BetType = BetType.AsianGoals,
            BetPick = BetPick.Over,
            Handicap = 1.5m,
            Wager = 200,
            TimeStamp = Now
        };

        private static readonly Guid BettorId = Guid.NewGuid();
        private static readonly Guid SeasonId = Guid.NewGuid();
        private static readonly Guid BookieId = Guid.NewGuid();

        private BetslipSubmitted _betslipSubmitted = new BetslipSubmitted
                                    {
                                        SourceId = BettorId,
                                        SeasonId = SeasonId,
                                        BookieId = BookieId,
                                        StartDate = new MonToSunWeek().FirstDayOfWeek(),
                                        EndDate = new MonToSunWeek().LastDayOfWeek(),
                                        BetslipId = Guid.NewGuid(),
                                        Bets = new List<Bet>
                                                   {
                                                       _bet1,
                                                       _bet2
                                                   }
                                    };
        [Fact]
        public void when_betslip_submitted_then_routes_and_saves()
        {
            var context = new StubProcessManagerDataContext<BookieProcess>();
            var router = new BookieProcessRouter(() => context);

            router.Handle(_betslipSubmitted);

            Assert.Equal(1, context.SavedProcesses.Count);
            Assert.True(context.DisposeCalled);
        }

        [Fact]
        public void when_betslip_submitted_is_reprocessed_then_routes_and_saves()
        {
            var bp = new BookieProcess
                         {
                             State = BookieProcess.ProcessState.Running,
                             BettorId = BettorId,
                             SeasonId = SeasonId,
                             BookieId = BookieId,
                         };
            
            var context = new StubProcessManagerDataContext<BookieProcess> { Store = { bp } };
            var router = new BookieProcessRouter(() => context);

            router.Handle(_betslipSubmitted);

            Assert.Equal(1, context.SavedProcesses.Count);
            Assert.True(context.DisposeCalled);
        }
    }

    class StubProcessManagerDataContext<T> : IProcessManagerDataContext<T> where T : class, IProcessManager
    {
        public readonly List<T> SavedProcesses = new List<T>();

        public readonly List<T> Store = new List<T>();

        public bool DisposeCalled { get; set; }

        public T Find(Guid id)
        {
            return Store.SingleOrDefault(x => x.Id == id);
        }

        public void Save(T processManager)
        {
            SavedProcesses.Add(processManager);
        }

        public T Find(Expression<Func<T, bool>> predicate, bool includeCompleted = false)
        {
            return Store.AsQueryable().Where(x => includeCompleted || !x.Completed).SingleOrDefault(predicate);
        }

        public void Dispose()
        {
            DisposeCalled = true;
        }
    }
}
