using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Ligi.Core.Model;
using Ligi.Core.Validation;

namespace Ligi.Infrastructure.Sql.Validation
{
    public class SaveLeagueValidator : IValidator<League>
    {
        public ValidationResultInfo Validate(League entity, DbContext context)
        {
            ValidationResultInfo vri = entity.BasicValidation();
            if (vri.Results.Any()) return vri;

            if (context.Set<League>()
                .Where(l => l.Id != entity.Id)
                .Any(l => l.Code == entity.Code))
            {
                vri.Results.Add(new ValidationResult("Duplicate league code."));
                return vri;
            }

            if (context.Set<League>()
                .Where(l => l.Id != entity.Id)
                .Any(l => l.Name == entity.Name))
                vri.Results.Add(new ValidationResult("Duplicate league name."));
            return vri;
        }
    }
}