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
            
            switch (ex)
            {
                case BaseException baseException:
                    context.Response.StatusCode = (int)baseException.StatusCode;
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            var errorResponse = new ErrorResponse()
            {
                Message = ex.Message
            };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}