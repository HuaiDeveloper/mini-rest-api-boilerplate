using System.Text.Json;
using MiniApi.Common;
using MiniApi.Common.Exceptions;

namespace MiniApi.Middleware;

public class ExceptionMiddleware : IMiddleware
{
    public ExceptionMiddleware()
    {
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            var errorResponse = new ErrorResponse()
            {
                Message = ex.Message
            };
            
            switch (ex)
            {
                case BaseException baseException:
                    context.Response.StatusCode = (int)baseException.StatusCode;
                    errorResponse.ErrorMessages = baseException.ErrorMessages;
                    break;

                default:
                    errorResponse.Message = "Internal server error";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}