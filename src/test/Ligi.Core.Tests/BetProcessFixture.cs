using System;
using System.Linq;
using Ligi.Core.Commands.Domain;
using Ligi.Core.Events.Domain;
using Ligi.Core.Model;
using Ligi.Core.Processes;
using Xunit;

namespace Ligi.Core.Tests
{
    public class BetContext
    {
        protected BetProcess _sut;
        protected static Guid fixtureId = Guid.NewGuid();

        public BetContext()
        {
            _sut = new BetProcess(fixtureId);
        }
    }

    public class when_bet_is_placed : BetContext
    {
        private BetPlaced _betPlaced;
        private Bet _bet1 = new Bet
        {
            FixtureId = fixtureId,
            BetType = BetType.AsianGoals,
            BetPick = BetPick.Away,
            Handicap = 1.5m,
            Wager = 200,
            Stake = 200,
            TimeStamp = DateTime.Now
        };

        public when_bet_is_placed()
        {
            _betPlaced = new BetPlaced
                             {
                                 SourceId = Guid.NewGuid(),
                                 UserId = Guid.NewGuid(),
                                 SeasonId = Guid.NewGuid(),
                                 Bet = _bet1
                             };

            _sut.Handle(_betPlaced);
        }

        [Fact]
        public void then_sends_add_bookie_command()
        {
            var addIndex = _sut.Commands.Select(x => x.Body).OfType<AddBookie>().Single();

            Assert.Equal(_betPlaced.SourceId, addIndex.BookieId);
            Assert.Equal(_sut.FixtureId,  addIndex.FixtureId);
        }
    }
}
