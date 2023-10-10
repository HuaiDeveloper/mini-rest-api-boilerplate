using System.Net;

namespace MiniApi.Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message, List<string>? errorMessages = null)
        : base(message, errorMessages, HttpStatusCode.Unauthorized)
    {
    }
}