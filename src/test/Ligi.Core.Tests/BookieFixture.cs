using System;
using System.Collections.Generic;
using Ligi.Core.Commands.Domain;
using Ligi.Core.Events.Domain;
using Ligi.Core.Handlers;
using Ligi.Core.Model;
using Ligi.Core.Services;
using Ligi.Core.Utils;
using Moq;
using Xunit;

namespace Ligi.Core.Tests
{
    public class BookieFixture
    {
        public class given_no_bets
        {
            private static readonly Guid BookieId = Guid.NewGuid();
            private static readonly Guid UserId = Guid.NewGuid();
            private static readonly Guid SeasonId = Guid.NewGuid();
            private static readonly DateTime StartDate = new MonToSunWeek().FirstDayOfWeek();
            private static readonly DateTime EndDate = new MonToSunWeek().LastDayOfWeek();
            private readonly EventSourcingTestHelper<Bookie> _sut;
            private readonly Mock<IPayoutService> _payoutService;
            private Bet _bet = new Bet
                                   {
                                       Id = Guid.NewGuid(),
                                       FixtureId = Guid.NewGuid(),
                                       BetType = BetType.AsianHandicap,
                                       BetPick = BetPick.Away,
                                       Handicap = 1.25m,
                                       Wager = 200,
                                       Stake = 200,
                                       TimeStamp = DateTime.Now
                                   };
            private static readonly PayoutResult PayoutResult = new PayoutResult {Payout = 195, Result = BetResult.Win};

            public given_no_bets()
            {
                _payoutService = new Mock<IPayoutService>();
                _payoutService.Setup(x => x.GetPayout(_bet, 0, 0)).Returns(PayoutResult);
                _sut = new EventSourcingTestHelper<Bookie>();
                _sut.Setup(new BookieCommandHandler(_sut.Repository, _payoutService.Object));
            }

            [Fact]
            public void when_processing_bets_then_right_events_are_generated()
            {
                var now = DateTime.Now;
                _sut.When(new ProcessBets
                              {
                                  SeasonId = SeasonId,
                                  UserId = UserId,
                                  BookieId = BookieId,
                                  StartDate = StartDate,
                                  EndDate = EndDate,
                                  BetslipId = Guid.NewGuid(),
                                  Bets = new List<Bet>
                                             {
                                                 new Bet
                                                     {
                                                         FixtureId = Guid.NewGuid(),
                                                         BetType = BetType.AsianHandicap,
                                                         BetPick = BetPick.Home,
                                                         Handicap = 1.5m,
                                                         Wager = 100,
                                                         TimeStamp = now
                                                     },

                                                 new Bet
                                                     {
                                                         FixtureId = Guid.NewGuid(),
                                                         BetType = BetType.AsianGoals,
                                                         BetPick = BetPick.Over,
                                                         Handicap = 1.5m,
                                                         Wager = 100,
                                                         TimeStamp = now
                                                     }
                                             }
                              });

                Assert.Equal(BookieId, _sut.ThenHasOne<WeekAccountOpened>().SourceId);
            }
        }

        public class given_at_least_two_successfully_placed_bets_for_the_season_week
        {
            private static readonly Guid BookieId = Guid.NewGuid();
            private static readonly Guid UserId = Guid.NewGuid();
            private static readonly Guid SeasonId = Guid.NewGuid();
            private static readonly DateTime StartDate = new MonToSunWeek().FirstDayOfWeek();
            private static readonly DateTime EndDate = new MonToSunWeek().LastDayOfWeek();
            private readonly EventSourcingTestHelper<Bookie> _sut;
            private readonly Mock<IPayoutService> _payoutService;
            private static readonly PayoutResult PayoutResult = new PayoutResult {Payout = 390, Result = BetResult.Win};
            private static readonly Guid BetslipId1 = Guid.NewGuid();
            private static readonly Guid FixtureId1 = Guid.NewGuid();
            private static readonly DateTime Now = DateTime.Now;

            private static Bet _bet1 = new Bet
                                           {
                                               Id = Guid.NewGuid(),
                                               FixtureId = FixtureId1,
                                               BetType = BetType.AsianHandicap,
                                               BetPick = BetPick.Away,
                                               Handicap = 1.25m,
                                               Wager = 200,
                                               Stake = 200,
                                               TimeStamp = Now
                                           };

            private static Bet _bet2 = new Bet
                                           {
                                               Id = Guid.NewGuid(),
                                               FixtureId = FixtureId1,
                                               BetType = BetType.AsianGoals,
                                               BetPick = BetPick.Over,
                                               Handicap = 1.5m,
                                               Wager = 200,
                                               Stake = 200,
                                               TimeStamp = Now
                                           };

            public given_at_least_two_successfully_placed_bets_for_the_season_week()
            {
                _payoutService = new Mock<IPayoutService>();
                _payoutService.Setup(x => x.GetPayout(_bet1, 0, 0)).Returns(PayoutResult);
                _payoutService.Setup(x => x.GetPayout(_bet2, 0, 0)).Returns(PayoutResult);

                _sut = new EventSourcingTestHelper<Bookie>();
                _sut.Setup(new BookieCommandHandler(_sut.Repository, _payoutService.Object));

                _sut.Given(new WeekAccountOpened
                               {
                                   SourceId = BookieId,
                                   UserId = UserId,
                                   StartDate = StartDate,
                                   EndDate = EndDate,
                                   SeasonId = SeasonId
                               },
                           new BetPlaced
                               {
                                   SourceId = BookieId,
                                   UserId = UserId,
                                   SeasonId = SeasonId,
                                   Bet = _bet1
                               },
                           new BetPlaced
                               {
                                   SourceId = BookieId,
                                   UserId = UserId,
                                   SeasonId = SeasonId,
                                   Bet = _bet2
                               },
                           new BetsProcessed
                               {
                                   SourceId = BookieId,
                                   BetslipId = BetslipId1,
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
            }

            [Fact]
            public void when_match_result_is_confirmed_then_bets_placed_for_the_fixture_are_transacted_correctly()
            {
                _sut.When(new TransactPayout
                              {
                                  BookieId = BookieId,
                                  FixtureId = FixtureId1,
                                  MatchStatus = MatchStatus.Played,
                                  HomeScore = 0,
                                  AwayScore = 0
                              });

                var events = _sut.Events;
            }
        }
    }
}
