using System.Net;

namespace MiniApi.Common.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message, List<string>? errorMessages = null) 
        : base(message, errorMessages, HttpStatusCode.NotFound)
    {
        
    }
}