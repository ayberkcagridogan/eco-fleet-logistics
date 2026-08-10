
using System.Net;
using FluentValidation.Results;


namespace EcoFleet.Shared.Kernel.Exceptions
{
    public class ValidationException : BaseException
    {
        public ValidationException(IDictionary<string, string[]> errors) 
            : base("One or more validation failures have occurred.", HttpStatusCode.BadRequest, errors)
        {
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
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