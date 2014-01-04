using System;
using System.Linq;
using Ligi.Core.Model;

namespace Ligi.Core.DataAccess
{
    public interface ISeasonRepository : IRepository<Season>
    {
        IQueryable<Season> GetByLeagueId(Guid leagueId);
        IQueryable<SeasonTeam> GetSeasonTeams();
        IQueryable<SeasonTeam> GetSeasonTeams(Guid seasonId);
    }
}
