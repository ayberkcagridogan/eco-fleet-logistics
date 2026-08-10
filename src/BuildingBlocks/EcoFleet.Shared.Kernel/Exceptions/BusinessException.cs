using System.Net;

namespace EcoFleet.Shared.Kernel.Exceptions;

public class BusinessException : BaseException
{
    public BusinessException(string message) 
        : base(message, HttpStatusCode.UnprocessableEntity)
    {
    }
}