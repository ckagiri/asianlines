using System;
using System.Collections.Generic;
using System.Linq;
using AsianLines.Core.Model;

namespace AsianLines.Core.DataAccess
{
    public interface ISeasonRepository : IRepository<Season>
    {
        IQueryable<Season> GetByLeagueId(Guid leagueId);
        IQueryable<SeasonTeam> GetSeasonTeams();
        IQueryable<SeasonTeam> GetSeasonTeams(Guid seasonId);
    }
}
