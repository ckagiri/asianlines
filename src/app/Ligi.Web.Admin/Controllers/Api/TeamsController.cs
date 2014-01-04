using System.Linq;
using System.Threading;
using System.Web.Http;
using Ligi.Core.Model;
using Ligi.Infrastructure.Sql.Database;

namespace Ligi.Web.Admin.Controllers.Api
{
    public class TeamsController : ApiController
    {
        public IQueryable<Team> Get()
        {
            Thread.Sleep(5000);
            var context = new AdminDbContext();
            var teams = context.Teams;
            return teams;
        }
    }
}
