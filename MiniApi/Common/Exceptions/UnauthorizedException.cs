using System.Net;

namespace MiniApi.Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message)
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}