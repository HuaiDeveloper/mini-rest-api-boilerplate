using System.Net;

namespace MiniApi.Common.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message) 
        : base(message, HttpStatusCode.NotFound)
    {
        
    }
}