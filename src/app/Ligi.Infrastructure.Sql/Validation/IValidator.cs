using System.Data.Entity;
using Ligi.Core.Validation;

namespace Ligi.Infrastructure.Sql.Validation
{
    public interface IValidator<in T> where T : class
    {
        ValidationResultInfo Validate(T entity, DbContext context);
    }
}
