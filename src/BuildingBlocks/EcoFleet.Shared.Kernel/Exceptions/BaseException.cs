
using System.Net;

namespace EcoFleet.Shared.Kernel.Exceptions
{
    public abstract class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public IDictionary<string, string[]>? Errors { get; }
    
        protected BaseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, IDictionary<string, string[]>? errors = null) 
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}