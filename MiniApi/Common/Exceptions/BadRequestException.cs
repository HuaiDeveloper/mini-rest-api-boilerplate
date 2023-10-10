using System.ComponentModel.DataAnnotations;
using System.Net;

namespace MiniApi.Common.Exceptions;

public class BadRequestException : BaseException
{
    public BadRequestException(string message, List<string>? errorMessages = null) 
        : base(message, errorMessages, HttpStatusCode.BadRequest)
    {
    }
    
    public BadRequestException(ICollection<ValidationResult> validationResult) 
        : base(
            "Bad request",
            validationResult.Count == 0 ? new List<string>()
                : validationResult
                    .Where(x => x.ErrorMessage != null)
                    .Select(x => x.ErrorMessage!)
                    .ToList(),
            HttpStatusCode.BadRequest)
    {
    }
    
    public BadRequestException(string message, ICollection<ValidationResult> validationResult) 
        : base(
            message,
            validationResult.Count == 0 ? new List<string>()
                : validationResult
                    .Where(x => x.ErrorMessage != null)
                    .Select(x => x.ErrorMessage!)
                    .ToList(),
            HttpStatusCode.BadRequest)
    {
    }
}