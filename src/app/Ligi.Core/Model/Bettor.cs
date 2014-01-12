using System;
using System.Collections.Generic;
using System.Linq;
using Ligi.Core.EventSourcing;
using Ligi.Core.Events;
using Ligi.Core.Events.Domain;

namespace Ligi.Core.Model
{
    public class Bettor : EventSourced
    {
        private string _userName;
        private readonly List<SeasonAccount> _seasonAccounts = new List<SeasonAccount>();
        private readonly Dictionary<Guid, List<BookieAlias>> _bookies = new Dictionary<Guid, List<BookieAlias>>();
        private readonly Dictionary<Guid, List<MonthAccount>> _monthAccounts = new Dictionary<Guid, List<MonthAccount>>(); 
        public Bettor(Guid id) : base(id)
        {
            Handles<BetslipSubmitted>(OnBetslipSubmitted);
            Handles<SeasonAccountUpdated>(OnSeasonAccountUpdated);
            Handles<MonthAccountUpdated>(OnMonthAccountUpdated);
        }

        public Bettor(Guid id, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            LoadFrom(history);
        }
       
        public void SubmitBetslip(string userName, Guid seasonId, List<BetItem> bets, DateTime timeStamp)
        {
            var seasonAccount = _seasonAccounts.FirstOrDefault(n => n.SeasonId == seasonId);
            if (seasonAccount == null)
            {
                seasonAccount = new SeasonAccount { Id = Guid.NewGuid(), SeasonId = seasonId };
            }
            var groups = bets
                .Select(item => new
                {
                    StartDate = item.StartOfWeek.Date,
                    EndDate = item.EndOfWeek.Date,
                    Bet = new Bet
                    {
                        FixtureId = item.FixtureId,
                        BetType = item.BetType,
                        Handicap = item.Handicap,
                        BetPick = item.BetPick,
                        Wager = Math.Round(item.Wager, 2),
                        MatchDay = item.FixtureKickOff.Date,
                        TimeStamp = timeStamp
                    }
                })
                .GroupBy(item => new { item.StartDate, item.EndDate }, data => data.Bet)
                .Select(wg => new WeekGroup
                {
                    StartDate = wg.Key.StartDate,
                    EndDate = wg.Key.EndDate,
                    Bets = wg.ToList()
                })
                .OrderBy(wg => wg.StartDate);

            List<BookieAlias> bookies;
            if(!_bookies.TryGetValue(seasonId, out bookies))
            {
                bookies = new List<BookieAlias>();
            }
            foreach (var group in groups)
            {
                WeekGroup wg = group;
                var bookie = bookies.FirstOrDefault(n => 
                    n.StartDate == wg.StartDate && n.EndDate == wg.EndDate);
                if (bookie == null)
                {
                    bookie = new BookieAlias
                                 {
                                     Id = Guid.NewGuid(),
                                     StartDate = wg.StartDate,
                                     EndDate = wg.EndDate
                                 };
                }

                Update(new BetslipSubmitted
                           {
                               BetslipId = Guid.NewGuid(),
                               UserName = userName,
                               SeasonId = seasonId,
                               BookieId = bookie.Id,
                               StartDate = bookie.StartDate,
                               EndDate = bookie.EndDate,
                               Bets = wg.Bets,
                               SeasonAccount = seasonAccount
                           });
            }
        }

        public void UpdateSeasonAccount(Guid seasonId, Guid bookieId, List<BetTransaction> betTransactions)
        {
            var seasonAccount = _seasonAccounts.First(n => n.SeasonId == seasonId);
            var monthAccounts = new List<MonthAccount>();
            foreach (var betTx in betTransactions)
            {
                var bet = betTx.Bet;
                List<MonthAccount> seasonMonthAccounts;
                if (!_monthAccounts.TryGetValue(seasonId, out seasonMonthAccounts))
                {
                    seasonMonthAccounts = new List<MonthAccount>();
                }
                var monthAccount = seasonMonthAccounts.FirstOrDefault(n => n.SeasonId == seasonId && n.Month == bet.MatchDay.Month);
                if (monthAccount == null)
                {
                    monthAccount = new MonthAccount
                                       {
                                           Id = Guid.NewGuid(),
                                           SeasonId = seasonId,
                                           Month = bet.MatchDay.Month,
                                           Year = bet.MatchDay.Year
                                       };
                }
                UpdateAccount(seasonAccount, bet, betTx.TxType);
                UpdateAccount(monthAccount, bet, betTx.TxType);
                monthAccounts.Add(monthAccount);
            }
            foreach (var monthAccount in monthAccounts)
            {
                monthAccount.UserName = _userName;
                Update(new MonthAccountUpdated
                           {
                               MonthAccount = monthAccount
                           });
                    
            }
            seasonAccount.UserName = _userName;
            Update(new SeasonAccountUpdated
                       {
                           BookieId = bookieId,
                           SeasonAccount = seasonAccount,
                       });
        }

        private void UpdateAccount(SeasonAccount account, Bet bet, TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.New:
                    account.TotalStake += bet.Wager;
                    if (bet.TimeStamp > account.LatestBetTimeStamp)
                    {
                        account.LatestBetTimeStamp = bet.TimeStamp;
                    }
                    account.BetsPlaced += 1;
                    break;
                case TransactionType.Update:
                    account.TotalStake += bet.Wager;
                    break;
                case TransactionType.Payout:
                    account.TotalPayout += bet.Payout;
                    account.BetsSettled += 1;
                    break;
            }
            account.Profit = account.TotalPayout - account.TotalStake;
        }

        private void OnBetslipSubmitted(BetslipSubmitted e)
        {
            List<BookieAlias> bookies;
            if (!_bookies.TryGetValue(e.SeasonId, out bookies))
            {
                bookies = new List<BookieAlias>();
                _bookies.Add(e.SeasonId, bookies);
            }

            if (!_bookies[e.SeasonId].Exists(n => n.Id == e.BookieId))
            {
                _bookies[e.SeasonId].Add(new BookieAlias
                                             {
                                                 Id = e.BookieId,
                                                 StartDate = e.StartDate,
                                                 EndDate = e.EndDate,
                                             });
            }

            if (!_seasonAccounts.Exists(n => Id == e.SeasonAccount.Id))
            {
                _seasonAccounts.Add(e.SeasonAccount);
            }

            _userName = e.UserName;
        }

        private void OnSeasonAccountUpdated(SeasonAccountUpdated e)
        {
            var seasonId = e.SeasonAccount.SeasonId;
            var seasonAccount = _seasonAccounts.First(n => n.SeasonId == seasonId);
            seasonAccount.TotalStake = e.SeasonAccount.TotalStake;
            seasonAccount.TotalPayout = e.SeasonAccount.TotalPayout;
            seasonAccount.Profit = e.SeasonAccount.Profit;
            seasonAccount.LatestBetTimeStamp = e.SeasonAccount.LatestBetTimeStamp;
            seasonAccount.BetsPlaced = e.SeasonAccount.BetsPlaced;
            seasonAccount.BetsSettled = e.SeasonAccount.BetsSettled;
        }

        private void OnMonthAccountUpdated(MonthAccountUpdated e)
        {
            List<MonthAccount> monthAccounts;
            var seasonId = e.MonthAccount.SeasonId;
            if (!_monthAccounts.TryGetValue(seasonId, out monthAccounts))
            {
                monthAccounts = new List<MonthAccount>();
                _monthAccounts.Add(seasonId, monthAccounts);
            }

            var existingMonthAccount = _monthAccounts[seasonId].FirstOrDefault(
                n => n.Month == e.MonthAccount.Month);
            if (existingMonthAccount == null)
            {
                _monthAccounts[seasonId].Add(e.MonthAccount);
            }
            else
            {
                existingMonthAccount.TotalStake = e.MonthAccount.TotalStake;
                existingMonthAccount.TotalPayout = e.MonthAccount.TotalPayout;
                existingMonthAccount.BetsPlaced = e.MonthAccount.BetsPlaced;
                existingMonthAccount.BetsSettled = e.MonthAccount.BetsSettled;
                existingMonthAccount.LatestBetTimeStamp = e.MonthAccount.LatestBetTimeStamp;
            }
        }

        public class WeekGroup
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<Bet> Bets { get; set; }
        }
    }
}
