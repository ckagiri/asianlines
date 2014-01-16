using Ligi.Core.DataAccess;
using Ligi.Core.Model;

namespace Ligi.Infrastructure.Sql.DataAccess
{
    public class TeamRepository : RepositoryBase<Team>, ITeamRepository
    {
        public TeamRepository(IUnitOfWork uow) : base(uow)
        { }
    }
}
