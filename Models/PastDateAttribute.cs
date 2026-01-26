using System.ComponentModel.DataAnnotations;

namespace BillingSystem.Models
{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime >= DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "Date must be in the past.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
