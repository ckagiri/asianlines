using Ligi.Core.DataAccess;
using Ligi.Core.Model;

namespace Ligi.Infrastructure.Sql.DataAccess
{
    public class LeagueRepository : RepositoryBase<League>, ILeagueRepository
    {
        public LeagueRepository(IUnitOfWork uow) : base(uow)
        { } 
    }
}
