using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Ligi.Core.Model;
using Ligi.Infrastructure.Sql.Database;

namespace Ligi.Web.Public.Controllers.Api
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
