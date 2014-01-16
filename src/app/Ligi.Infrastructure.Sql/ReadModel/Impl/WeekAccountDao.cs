using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ligi.Infrastructure.Sql.Database;
using Ligi.Infrastructure.Sql.ReadModel.Contracts;

namespace Ligi.Infrastructure.Sql.ReadModel.Impl
{
    public class WeekAccountDao : IWeekAccountDao
    {
        private readonly Func<BetsDbContext> _contextFactory;

        public WeekAccountDao(Func<BetsDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public WeekAccount GetWeekAccount(Guid userId, DateTime startDate, DateTime endDate)
        {
            using (var context = _contextFactory.Invoke())
            {
                var account = context.Query<WeekAccount>()
                    .FirstOrDefault(n => 
                        n.UserId == userId && n.StartDate == startDate && n.EndDate == endDate);

                return account;
            }
        }

        public List<WeekAccount> GetWeekAccounts(DateTime startDate, DateTime endDate)
        {
            using (var context = _contextFactory.Invoke())
            {
                var accounts = context.Query<WeekAccount>()
                    .Where(n => n.StartDate == startDate && n.EndDate == endDate)
                    .Take(10)
                    .ToList();

                return accounts;
            }
        }

        public List<WeekAccount> GetWeekAccounts(Guid userId)
        {
            using (var context = _contextFactory.Invoke())
            {
                var accounts = context.Query<WeekAccount>()
                    .Where(n => n.UserId == userId)
                    .Take(10)
                    .ToList();

                return accounts;
            }
        }

        public async Task<WeekAccount> GetWeekAccountAsync(Guid userId, DateTime startDate, DateTime endDate)
        {
            var weekAccount = await Task.Factory.StartNew(
                () => GetWeekAccount(userId, startDate, endDate));
            return weekAccount;
        }

        public async Task<List<WeekAccount>> GetWeekAccountsAsync(Guid userId)
        {
            var weekAccounts = await Task.Factory.StartNew(
                () => GetWeekAccounts(userId));
            return weekAccounts;
        }
    }
}
