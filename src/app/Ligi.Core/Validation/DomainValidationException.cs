using System;

namespace Ligi.Core.Validation
{
    public class DomainValidationException : Exception
    {
        public DomainValidationException(ValidationResultInfo validationResults)
        {
            ValidationResults = validationResults;
        }

        public ValidationResultInfo ValidationResults { get; set; }
    }

}
