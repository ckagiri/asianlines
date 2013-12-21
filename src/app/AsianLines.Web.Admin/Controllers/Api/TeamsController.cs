using System.Linq;
using System.Threading;
using System.Web.Http;
using AsianLines.Core.Model;
using AsianLines.Infrastructure.Sql.Database;

namespace AsianLines.Web.Admin.Controllers.Api
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
