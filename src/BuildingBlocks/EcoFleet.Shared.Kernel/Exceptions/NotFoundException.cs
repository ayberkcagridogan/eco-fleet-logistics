
using System.Net;

namespace EcoFleet.Shared.Kernel.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(string name, object key) 
        : base($"Entity \"{name}\" ({key}) was not found.", HttpStatusCode.NotFound)
        {
        }

        public NotFoundException(string message) 
            : base(message, HttpStatusCode.NotFound)
        {
        }
    }
}