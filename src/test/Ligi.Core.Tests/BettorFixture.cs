using System;
using System.Collections.Generic;
using Ligi.Core.Commands.Domain;
using Ligi.Core.Events.Domain;
using Ligi.Core.Handlers;
using Ligi.Core.Model;
using Ligi.Core.Utils;
using Xunit;

namespace Ligi.Core.Tests
{
    public class given_no_betslip
    {
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid SeasonId = Guid.NewGuid();

        private readonly EventSourcingTestHelper<Bettor> _sut;

        public given_no_betslip()
        {
            _sut = new EventSourcingTestHelper<Bettor>();
            _sut.Setup(new BettorCommandHandler(_sut.Repository));
        }

        [Fact]
        public void when_submitting_betslip_then_is_placed_to_respective_bookies()
        {
            _sut.When(new SubmitBetslip
                          {
                              UserId = UserId,
                              SeasonId = SeasonId,
                              Bets = new List<BetItem>
                                         {
                                             new BetItem
                                                 {
                                                     StartOfWeek = new MonToSunWeek().Start(),
                                                     EndOfWeek = new MonToSunWeek().End(),
                                                     FixtureId = Guid.NewGuid(),
                                                     BetType = BetType.AsianGoals,
                                                     BetPick = BetPick.Over,
                                                     Handicap = 1.5m,
                                                     Wager = 120
                                                 },
                                             new BetItem
                                                 {
                                                     StartOfWeek = new MonToSunWeek().Start().AddDays(7),
                                                     EndOfWeek = new MonToSunWeek().End().AddDays(7),
                                                     FixtureId = Guid.NewGuid(),
                                                     BetType = BetType.AsianHandicap,
                                                     BetPick = BetPick.Away,
                                                     Handicap = -1.25m,
                                                     Wager = 180
                                                 },
                                             new BetItem
                                                 {
                                                     StartOfWeek = new MonToSunWeek().Start(),
                                                     EndOfWeek = new MonToSunWeek().End(),
                                                     FixtureId = Guid.NewGuid(),
                                                     BetType = BetType.AsianHandicap,
                                                     BetPick = BetPick.Home,
                                                     Handicap = 2.0m,
                                                     Wager = 250
                                                 }
                                         },
                                         TimeStamp = DateTime.Now
                          });

            Assert.Equal(2, _sut.Events.Count);
        }
    }

    public class given_atleast_one_successfully_placed_betslip_for_the_season
    {
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid SeasonId = Guid.NewGuid();
        private static Bet _bet1;
        private static Bet _bet2;
        private static readonly Guid BookieId = Guid.NewGuid();
        private DateTime now = DateTime.Now;

        private readonly EventSourcingTestHelper<Bettor> _sut;

        public given_atleast_one_successfully_placed_betslip_for_the_season()
        {
            _sut = new EventSourcingTestHelper<Bettor>();
            _sut.Setup(new BettorCommandHandler(_sut.Repository));

            _bet1 = new Bet
                       {
                           FixtureId = Guid.NewGuid(),
                           BetType = BetType.AsianHandicap,
                           BetPick = BetPick.Away,
                           Handicap = 1.5m,
                           Wager = 200,
                           TimeStamp = now
                       };
            _bet2 = new Bet
                       {
                           FixtureId = Guid.NewGuid(),
                           BetType = BetType.AsianGoals,
                           BetPick = BetPick.Over,
                           Handicap = 1.5m,
                           Wager = 200,
                           TimeStamp = now
                       };

            _sut.Given(new BetslipSubmitted
                           {
                               SourceId = UserId,
                               BookieId = BookieId,
                               SeasonId = SeasonId,
                               StartDate = new MonToSunWeek().FirstDayOfWeek(),
                               EndDate = new MonToSunWeek().LastDayOfWeek(),
                               BetslipId = Guid.NewGuid(),
                               Bets = new List<Bet>
                                         {
                                             _bet1,
                                             _bet2
                                         },
                           });
        }

        [Fact]
        public void when_updating_season_account_then_season_account_is_updated_accordingly()
        {
            _bet1.Id = Guid.NewGuid();
            _bet1.Stake = _bet1.Wager;
            _bet2.Id = Guid.NewGuid();
            _bet2.Stake = _bet2.Stake;
            _sut.When(new UpdateSeasonAccount
                          {
                              BettorId = UserId,
                              SeasonId = SeasonId,
                              BookieId = BookieId,
                              BetTransactions = new List<BetTransaction>
                                                    {
                                                        new BetTransaction
                                                            {
                                                                TxType = TransactionType.New,
                                                                Bet = _bet1
                                                            },
                                                        new BetTransaction
                                                        {
                                                            TxType = TransactionType.New,
                                                            Bet = _bet2
                                                        }
                                                    }
                          });
            
            var @event = _sut.ThenHasOne<SeasonAccountUpdated>();
        }
    }
}
