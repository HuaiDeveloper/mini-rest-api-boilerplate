using System.Net;

namespace MiniApi.Common.Exceptions;

public class BadRequestException : BaseException
{
    public BadRequestException(string message) 
        : base(message, HttpStatusCode.BadRequest)
    {
        
    }
}