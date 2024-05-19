using System.ComponentModel.DataAnnotations;

namespace MiniApi.Common;

public static class CustomValidator
{
    public static bool TryValidateObject<T>(T instance, out ICollection<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        
        if (instance == null) return false;

        return Validator.TryValidateObject(
            instance,
            new ValidationContext(instance),
            validationResults,
            true);
    }
}