using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using BillingSystem.Models;

namespace BillingSystem.ViewModels
{
    public class CashierBillingViewModel
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        public string PatientName { get; set; } = string.Empty;
        
        public int Age { get; set; }
        
        public bool IsSenior { get; set; }
        
        public bool IncludeConsultationFee { get; set; } = true;

        public List<int> SelectedServiceIds { get; set; } = new List<int>();
        public List<int> SelectedLabOrderIds { get; set; } = new List<int>();
        public List<int> SelectedPrescriptionIds { get; set; } = new List<int>();

        public List<SelectListItem> AvailableServices { get; set; } = new List<SelectListItem>();

        [Range(0, 365, ErrorMessage = "Admit days must be between 0 and 365")]
        public int AdmitDays { get; set; }

        public bool HasInsurance { get; set; }

        [RequiredIfTrue(nameof(HasInsurance), ErrorMessage = "Insurance Coverage Type is required if Has Insurance is checked")]
        public InsuranceCoverageType? InsuranceCoverageType { get; set; }

        [RequiredIfTrue(nameof(HasInsurance), ErrorMessage = "Insurance % is required if Has Insurance is checked")]
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
        public double? InsurancePercent { get; set; }

        [RequiredIfTrue(nameof(HasInsurance), ErrorMessage = "Provider Name is required if Has Insurance is checked")]
        public string? ProviderName { get; set; }

        [RequiredIfTrue(nameof(HasInsurance), ErrorMessage = "Policy Number is required if Has Insurance is checked")]
        public string? PolicyNumber { get; set; }

        public List<SelectListItem> AvailableProviders { get; set; } = new();

        public BillPreviewViewModel? BillPreview { get; set; }
    }
    
    // Custom Validation Attribute for Conditional Required
    public class RequiredIfTrueAttribute : ValidationAttribute
    {
        private readonly string _propertyName;

        public RequiredIfTrueAttribute(string propertyName)
        {
            _propertyName = propertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var propertyValue = type.GetProperty(_propertyName)?.GetValue(instance, null);

            if (propertyValue is bool boolValue && boolValue)
            {
                 if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                 {
                     return new ValidationResult(ErrorMessage ?? $"The field is required.");
                 }
            }
            return ValidationResult.Success;
        }
    }
}
