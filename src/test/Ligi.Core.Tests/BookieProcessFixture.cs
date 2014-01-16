using System;
using System.Collections.Generic;
using System.Linq;
using Ligi.Core.Commands.Domain;
using Ligi.Core.Events.Domain;
using Ligi.Core.Model;
using Ligi.Core.Processes;
using Ligi.Core.Utils;
using Xunit;

namespace Ligi.Core.Tests
{
    namespace given_uninitialized_process
    {
        public class Context
        {
            protected BookieProcess _sut;
            public Context()
            {
                _sut = new BookieProcess();
            }
        }

        public class when_betslip_is_submitted : Context
        {
            private BetslipSubmitted _betslipSubmitted;
            private static readonly DateTime Now = DateTime.Now;
            private Bet _bet1 = new Bet
                                    {
                                        FixtureId = Guid.NewGuid(),
                                        BetType = BetType.AsianHandicap,
                                        BetPick = BetPick.Away,
                                        Handicap = 1.5m,
                                        Wager = 200,
                                        TimeStamp = Now
                                    };
            private Bet _bet2 = new Bet
                                    {
                                        FixtureId = Guid.NewGuid(),
                                        BetType = BetType.AsianGoals,
                                        BetPick = BetPick.Over,
                                        Handicap = 1.5m,
                                        Wager = 200,
                                        TimeStamp = Now
                                    };
            public when_betslip_is_submitted()
            {
                _betslipSubmitted = new BetslipSubmitted
                                        {
                                            SourceId = Guid.NewGuid(),
                                            BetslipId = Guid.NewGuid(),
                                            SeasonId = Guid.NewGuid(),
                                            BookieId = Guid.NewGuid(),
                                            StartDate = new MonToSunWeek().FirstDayOfWeek(),
                                            EndDate = new MonToSunWeek().LastDayOfWeek(),
                                            Bets = new List<Bet>
                                                       {
                                                           _bet1,
                                                           _bet2
                                                       }
                                        };
                _sut.Handle(_betslipSubmitted);
            }

            [Fact]
            public void then_sends_process_bets_command()
            {
                Assert.Equal(1, _sut.Commands.Count());
            }

            [Fact]
            public void then_process_bets_is_sent_for_specific_betslip()
            {
                var betslipSubmittion = _sut.Commands.Select(x => x.Body).OfType<ProcessBets>().Single();
                Assert.Equal(_betslipSubmitted.BetslipId, betslipSubmittion.BetslipId);
            }

            [Fact]
            public void then_transitions_to_running_state()
            {
                Assert.Equal(BookieProcess.ProcessState.Running, _sut.State);
            }
        }
    }

    namespace given_process_running
    {
        public class Context
        {
            protected BookieProcess _sut;
            protected Guid BettorId;
            protected Guid SeasonId;
            protected Guid BookieId;
            protected Bet Bet1;
            protected Bet Bet2;
            protected Guid Id;
            protected Guid FixtureId1;
            protected Guid FixtureId2;
            protected static DateTime Now = DateTime.Now;
            public Context()
            {
                _sut = new BookieProcess();
                BettorId = Guid.NewGuid();
                SeasonId = Guid.NewGuid();
                BookieId = Guid.NewGuid();
                FixtureId1 = Guid.NewGuid();
                FixtureId2 = Guid.NewGuid();
                Bet1 = new Bet
                           {
                               FixtureId = FixtureId1,
                               BetType = BetType.AsianHandicap,
                               BetPick = BetPick.Away,
                               Handicap = 1.5m,
                               Wager = 200,
                               TimeStamp = Now
                           };
                Bet2 = new Bet
                           {
                               FixtureId = FixtureId2,
                               BetType = BetType.AsianGoals,
                               BetPick = BetPick.Over,
                               Handicap = 1.5m,
                               Wager = 200,
                               TimeStamp = Now
                           };

                _sut.Handle(new BetslipSubmitted
                                {
                                    SourceId = BettorId,
                                    BetslipId = Guid.NewGuid(),
                                    SeasonId = SeasonId,
                                    BookieId = BookieId,
                                    StartDate = new MonToSunWeek().FirstDayOfWeek(),
                                    EndDate = new MonToSunWeek().LastDayOfWeek(),
                                    Bets = new List<Bet>
                                               {
                                                   Bet1,
                                                   Bet2
                                               }
                                });
                Bet1.Stake = Bet1.Wager;
                Bet2.Stake = Bet2.Wager;
                _sut.Handle(new BetsProcessed
                                {
                                    SourceId = BettorId,
                                    BetslipId = Guid.NewGuid(),
                                    BetTransactions = new List<BetTransaction>
                                                          {
                                                              new BetTransaction
                                                                  {
                                                                      TxType = TransactionType.New,
                                                                      Bet = Bet1
                                                                  },
                                                              new BetTransaction
                                                                  {
                                                                      TxType = TransactionType.New,
                                                                      Bet = Bet2
                                                                  }
                                                          }
                                });

                // season acc updtd

            }


            [Fact]
            public void then_transitions_state()
            {
                Assert.Equal(BookieProcess.ProcessState.AwaitingSeasonAccountAck, _sut.State);
            }
        }
    }
}
