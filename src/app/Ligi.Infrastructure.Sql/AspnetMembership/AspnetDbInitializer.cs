using System.Data.Entity;

namespace Ligi.Infrastructure.Sql.AspnetMembership
{
    public class AspnetDbInitializer : 
        CreateDatabaseIfNotExists<AspnetDbContext>
    {
    }
}
