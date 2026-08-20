
using System.Net;
using FluentValidation.Results;


namespace EcoFleet.Shared.Kernel.Exceptions
{
    public class CustomValidationException : BaseException
    {
        public CustomValidationException(IDictionary<string, string[]> errors) 
            : base("One or more validation failures have occurred.", HttpStatusCode.BadRequest, errors)
        {
        }

        public CustomValidationException(IEnumerable<ValidationFailure> failures)
        : base( "One or more validation failures have occurred.", 
            HttpStatusCode.BadRequest, 
            ConvertFailuresToDictionary(failures))
        {
        }

        private static IDictionary<string, string[]>? ConvertFailuresToDictionary(IEnumerable<ValidationFailure> failures)
        {
            return failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(
                    failureGroup => failureGroup.Key, 
                    failureGroup => failureGroup.ToArray()
                );
        }
    }
}