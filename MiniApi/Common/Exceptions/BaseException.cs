using System.Net;

namespace MiniApi.Common.Exceptions;

public class BaseException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public List<string>? ErrorMessages { get; set; }

    public BaseException(string message, List<string>? errorMessages = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        ErrorMessages = errorMessages;
        StatusCode = statusCode;
    }
}