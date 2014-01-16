using Ligi.Core.DataAccess;
using Ligi.Core.Model;

namespace Ligi.Infrastructure.Sql.DataAccess
{
    public class SeasonRepository : RepositoryBase<Season>, ISeasonRepository
    {
        public SeasonRepository(IUnitOfWork uow) : base(uow)
        { }
    }
}
