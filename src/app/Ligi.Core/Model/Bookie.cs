using System;
using System.Collections.Generic;
using System.Linq;
using Ligi.Core.EventSourcing;
using Ligi.Core.Events;
using Ligi.Core.Events.Domain;
using Ligi.Core.Services;

namespace Ligi.Core.Model
{
    public class Bookie : EventSourced
    {
        private readonly List<Bet> _bets = new List<Bet>();
        private readonly List<Guid> _betSlipIds = new List<Guid>();

        public Bookie(Guid id) : base(id)
        {
            Handles<WeekAccountOpened>(OnWeekAccountOpened);
            Handles<BetPlaced>(OnBetPlaced);
            Handles<BetUpdated>(OnBetUpdated);
            Handles<BetRejected>(OnBetRejected);
            Handles<BetUpdateRejected>(OnBetUpdateRejected);
            Handles<PayoutTransacted>(OnPayoutTransacted);
            Handles<BetsProcessed>(OnBetsProcessed);
        }

        public Bookie(Guid id, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            LoadFrom(history);
        }

        public Bookie(Guid id, DateTime startOfWeek, DateTime endOfWeek, Guid userId, Guid seasonId)
            : this(id)
        {
            Update(new WeekAccountOpened { UserId = userId, StartDate = startOfWeek, EndDate = endOfWeek,  SeasonId = seasonId});
        }

        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public Guid SeasonId { get; set; }
        public void ProcessBets(Guid betslipId, string userName, IEnumerable<Bet> bets)
        {
            if (_betSlipIds.Contains(betslipId))
            {
                return;
            }
            var betItems = bets.ToList();
            var betTransactions = new List<BetTransaction>();
            foreach (var betItem in betItems)
            {
                var existing = _bets.FirstOrDefault(n => n.FixtureId == betItem.FixtureId && 
                    n.BetType == betItem.BetType && n.Handicap == betItem.Handicap);
                if (existing == null)
                {
                    if (betItem.Wager > Credit + Balance)
                    {
                        Update(new BetRejected { UserId = UserId, Bet = betItem });
                    }
                    else
                    {
                        if (betItem.Wager < 50)
                        {
                            Update(new BetRejected { UserId = UserId, Bet = betItem });
                        }
                        else
                        {
                            betItem.Id = Guid.NewGuid();
                            betItem.Stake = betItem.Wager;
                            betTransactions.Add(new BetTransaction { TxType = TransactionType.New, Bet = betItem });
                            Update(new BetPlaced { UserId = UserId, UserName = userName, Bet = betItem, SeasonId = SeasonId });
                        }
                    }
                }
                else
                {
                    if (betItem.Wager > Credit + Balance)
                    {
                        Update(new BetUpdateRejected { UserId = UserId, Bet = betItem });
                    }
                    else
                    {
                        if (betItem.Wager < 50)
                        {
                            Update(new BetUpdateRejected { UserId = UserId, Bet = betItem });
                        }
                        else
                        {
                            existing.Wager = betItem.Wager;
                            existing.Stake += betItem.Wager;
                            betTransactions.Add(new BetTransaction { TxType = TransactionType.Update, Bet = existing });
                            Update(new BetUpdated { UserId = UserId, UserName = userName, Bet = existing });
                        }
                    }
                }
            }

            if (betTransactions.Any())
            {
                Update(new BetsProcessed { BetslipId = betslipId, BetTransactions = betTransactions });
            }
        }

        public void TransactPayout(Guid fixtureId, MatchStatus matchStatus, int homeScore, int awayScore, IPayoutService payoutService)
        {
            var betTransactions = new List<BetTransaction>();
            var bets = _bets.Where(n => n.FixtureId == fixtureId && n.Payout == 0).ToArray();
            foreach (var bet in bets)
            {
                BetResult betResult;
                if (matchStatus == MatchStatus.Played)
                {
                    var payoutResult = payoutService.GetPayout(bet, homeScore, awayScore);
                    bet.Payout = payoutResult.Payout;
                    betResult = payoutResult.Result;
                }
                else
                {
                    bet.Payout = bet.Stake;
                    betResult = BetResult.Void;
                }
                bet.Profit = bet.Payout - bet.Stake;
                betTransactions.Add(new BetTransaction { TxType = TransactionType.Payout, Bet = bet });
                Update(new PayoutTransacted { UserId = UserId, Bet = bet, BetResult = betResult});
            }

            if (betTransactions.Any())
            {
                Update(new BetsProcessed { BetTransactions = betTransactions });
            }
        }

        private void OnWeekAccountOpened(WeekAccountOpened e)
        {
            SeasonId = e.SeasonId;
            UserId = e.UserId;
            StartDate = e.StartDate;
            EndDate = e.EndDate;
            Credit = 1000;
        }

        private void OnBetPlaced(BetPlaced e)
        {
            UpdateAvailableCash(e.Bet.Wager);
            _bets.Add(e.Bet);
        }

        private void OnBetUpdated(BetUpdated e)
        {
            UpdateAvailableCash(e.Bet.Wager);
            var bet = _bets.First(n => n.Id == e.Bet.Id);
            bet.Stake = e.Bet.Stake;
        }

        private void UpdateAvailableCash(decimal wager)
        {
            if (Credit > wager)
            {
                Credit -= wager;
            }
            else
            {
                var diff = wager - Credit;
                Credit = 0;
                Balance -= diff;
            }
        }

        private void OnPayoutTransacted(PayoutTransacted e)
        {
            Balance += e.Bet.Payout;
            var bet = _bets.First(n => n.Id == e.Bet.Id);
            bet.Payout = e.Bet.Payout;
        }

        private void OnBetRejected(BetRejected e)
        {
        }

        private void OnBetUpdateRejected(BetUpdateRejected e)
        {
        }

        private void OnBetsProcessed(BetsProcessed e)
        {
            if (e.BetslipId != Guid.Empty)
            {
                _betSlipIds.Add(e.BetslipId);
            }
        }
    }
}
