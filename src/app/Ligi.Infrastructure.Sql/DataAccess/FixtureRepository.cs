using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Ligi.Core.DataAccess;
using Ligi.Core.Model;

namespace Ligi.Infrastructure.Sql.DataAccess
{
    public class FixtureRepository : RepositoryBase<Fixture>, IFixtureRepository
    {
        public FixtureRepository(IUnitOfWork uow) : base(uow)
        { }

        public IEnumerable<Fixture> FindAll(Expression<Func<Fixture, bool>> predicate = null, 
            bool includeTeams = false)
        {
            return Query.Include("HomeTeam").Include("AwayTeam").ToArray();
        }

        public async Task<Fixture> FindAsync(Guid id)
        {
            var fixture =  await Task.Factory.StartNew(() => DbSet.Find(id));
            return fixture;
        }

        public void Update(Fixture existing, Fixture update)
        {
            DbContext.Entry(existing).CurrentValues.SetValues(update);
        }
    }
}
