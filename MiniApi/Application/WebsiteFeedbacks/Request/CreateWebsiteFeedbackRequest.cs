using System.ComponentModel.DataAnnotations;
using MiniApi.Common.Enum;

namespace MiniApi.Application.WebsiteFeedbacks.Request;

public class CreateWebsiteFeedbackRequest : IValidatableObject
{
    [Required]
    public WebsiteFeedbackTypeEnum Type { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Content { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Enum.IsDefined(typeof(WebsiteFeedbackTypeEnum), Type) == false)
        {
            yield return new ValidationResult(
                $"feedback type does not exist");
        }
    }
}