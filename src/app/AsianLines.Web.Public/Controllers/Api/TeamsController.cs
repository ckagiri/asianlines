using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AsianLines.Core.Model;
using AsianLines.Infrastructure.Sql.Database;

namespace AsianLines.Web.Public.Controllers.Api
{
    public class TeamsController : ApiController
    {
        public IEnumerable<Team> Get()
        {
            var context = new AdminDbContext();
            var teams = context.Teams.ToList();
            return teams;
        }
    }
}
