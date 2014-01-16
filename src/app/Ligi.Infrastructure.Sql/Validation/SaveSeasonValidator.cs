using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Ligi.Core.Model;
using Ligi.Core.Validation;

namespace Ligi.Infrastructure.Sql.Validation
{
    public class SaveSeasonValidator : IValidator<Season>
    {
        public ValidationResultInfo Validate(Season entity, DbContext context)
        {
            ValidationResultInfo vri = entity.BasicValidation();
            if (vri.Results.Any()) return vri;

            if (entity.LeagueId == Guid.Empty)
            {
                vri.Results.Add(new ValidationResult("League is required."));
                return vri;
            }
            return vri;
        }
    }
}
