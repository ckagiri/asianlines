using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ligi.Core.Validation
{
    public static class ValidationExtensions
    {
        public static ValidationResultInfo BasicValidation<T>(this T itemToValidate)
        {
            var vc = new ValidationContext(itemToValidate, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(itemToValidate, vc, results, true);

            return new ValidationResultInfo { Results = results };
        }
    }
}
