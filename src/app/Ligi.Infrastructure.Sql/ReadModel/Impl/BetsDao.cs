using System;
using System.Collections.Generic;
using System.Linq;
using Ligi.Infrastructure.Sql.Database;
using Ligi.Infrastructure.Sql.ReadModel.Contracts;

namespace Ligi.Infrastructure.Sql.ReadModel.Impl
{
    public class BetsDao : IBetsDao
    {
        private readonly Func<BetsDbContext> _contextFactory;

        public BetsDao(Func<BetsDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<Bet> GetBets(Guid userId)
        {
            using (var context = _contextFactory.Invoke())
            {
                var bets = context.Query<Bet>()
                    .Where(n => n.UserId == userId)
                    .ToList();

                return bets;
            }
        }

        public List<Bet> GetBets(Guid userId, Guid seasonId)
        {
            using (var context = _contextFactory.Invoke())
            {
                var bets = context.Query<Bet>()
                    .Where(n => n.UserId == userId && n.SeasonId == seasonId)
                    .ToList();

                return bets;
            }
        }

        public List<Bet> GetBets(Guid userId, Guid seasonId, DateTime startDate, DateTime endDate)
        {
            using (var context = _contextFactory.Invoke())
            {
                var bets = context.Query<Bet>()
                    .Where(n => n.UserId == userId && n.SeasonId == seasonId
                        && startDate >= n.TimeStamp && n.TimeStamp <= endDate)
                    .ToList();

                return bets;
            }
        }
    }
}
