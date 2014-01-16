using System;
using System.Collections.Generic;
using System.Linq;
using Ligi.Infrastructure.Sql.Database;
using Ligi.Infrastructure.Sql.ReadModel.Contracts;

namespace Ligi.Infrastructure.Sql.ReadModel.Impl
{
    public class LeaderBoardDao : ILeaderBoardDao
    {
        private readonly Func<BetsDbContext> _contextFactory;

        public LeaderBoardDao(Func<BetsDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public WeekAccount GetWeekEntry(Guid seasonId, Guid userId, DateTime startDate, DateTime endDate)
        {
            using (var context = _contextFactory.Invoke())
            {
                var entry = context.Query<WeekAccount>()
                    .FirstOrDefault(n => n.SeasonId == seasonId && n.UserId == userId
                        && n.StartDate == startDate && n.EndDate == endDate);

                return entry;
            }
        }

        public MonthLeaderBoard GetMonthEntry(Guid seasonId, Guid userId, int month)
        {
            using (var context = _contextFactory.Invoke())
            {
                var entry = context.Query<MonthLeaderBoard>()
                    .FirstOrDefault(n => n.SeasonId == seasonId && n.UserId == userId 
                        && n.Month == month);

                return entry;
            }
        }

        public SeasonLeaderBoard GetSeasonEntry(Guid seasonId, Guid userId)
        {
            using (var context = _contextFactory.Invoke())
            {
                var entry = context.Query<SeasonLeaderBoard>()
                    .FirstOrDefault(n => n.SeasonId == seasonId && n.UserId == userId);

                return entry;
            }
        }

        public List<WeekAccount> GetWeekEntries(Guid seasonId, DateTime startDate, DateTime endDate)
        {
            using (var context = _contextFactory.Invoke())
            {
                var entries = context.Query<WeekAccount>()
                    .Where(n => n.SeasonId == seasonId && n.StartDate == startDate && n.EndDate == endDate)//&& n.Profit > 0)
                    .OrderByDescending(n => n.Profit)
                    .Take(10)
                    .ToList();

                return entries;
            }
        }

        public List<MonthLeaderBoard> GetMonthEntries(Guid seasonId, int month)
        {
            using (var context = _contextFactory.Invoke())
            {
                var entries = context.Query<MonthLeaderBoard>()
                    .Where(n => n.SeasonId == seasonId && n.Month == month )//&& n.Profit > 0)
                    .OrderByDescending(n => n.Profit)
                    .Take(10)
                    .ToList();

                return entries;
            }
        }

        public List<SeasonLeaderBoard> GetSeasonEntries(Guid seasonId)
        {
            using (var context = _contextFactory.Invoke())
            {
                var entries = context.Query<SeasonLeaderBoard>()
                    .Where(n => n.SeasonId == seasonId )//&& n.Profit > 0)
                    .OrderByDescending(n => n.Profit)
                    .Take(10)
                    .ToList();

                return entries;
            }
        }
    }
}
