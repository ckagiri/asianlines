using System;
using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.ReadModel.Contracts
{
    public interface IWeekAccountDao
    {
        WeekAccount GetWeekAccount(Guid userId, DateTime startDate, DateTime endDate);
        List<WeekAccount> GetWeekAccounts(Guid userId);
        List<WeekAccount> GetWeekAccounts(DateTime startDate, DateTime endDate);
    }
}
