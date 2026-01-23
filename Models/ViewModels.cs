using BillingSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.ViewModels
{
    public class PatientRegistrationVM
    {
        [Required]
        [Display(Name = "Patient Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, 150)]
        public int Age { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        // Admission
        [Display(Name = "Admit Days (Default 0 if not admitted)")]
        [Range(0, 365)]
        public int AdmitDays { get; set; }
    }
}
