using System.Linq;
using System.Web.Mvc;
using Ligi.Infrastructure.Sql.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ligi.Web.Admin.Controllers.Mvc
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new AdminDbContext();
            var leagues = context.Leagues.ToArray();
            var seasons = context.Seasons.ToArray();
            var bsData = new { leagues, seasons };

            var jsonSettings = new JsonSerializerSettings
                               {
                                   ContractResolver = new CamelCasePropertyNamesContractResolver()
                               };
            var jsonObj = (object)JsonConvert.SerializeObject(bsData, Formatting.None, jsonSettings);

            return View("Index", jsonObj);
        }
    }
}
