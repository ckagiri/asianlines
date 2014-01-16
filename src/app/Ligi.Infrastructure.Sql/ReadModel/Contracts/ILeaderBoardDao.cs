using System;
using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.ReadModel.Contracts
{
    public interface ILeaderBoardDao
    {
        WeekAccount GetWeekEntry(Guid seasonId, Guid userId, DateTime startDate, DateTime endDate);
        MonthLeaderBoard GetMonthEntry(Guid seasonId, Guid userId, int month);
        SeasonLeaderBoard GetSeasonEntry(Guid seasonId, Guid userId);
        List<WeekAccount> GetWeekEntries(Guid seasonId, DateTime startDate, DateTime endDate);
        List<MonthLeaderBoard> GetMonthEntries(Guid seasonId, int month);
        List<SeasonLeaderBoard> GetSeasonEntries(Guid seasonId);
    }
}
