using System;
using System.Collections.Generic;

namespace Ligi.Infrastructure.Sql.ReadModel.Contracts
{
    public interface IBetsDao
    {
        List<Bet> GetBets(Guid userId, Guid seasonId, DateTime startDate, DateTime endDate);
        List<Bet> GetBets(Guid userId, Guid seasonId);
        List<Bet> GetBets(Guid userId);
    }
}
