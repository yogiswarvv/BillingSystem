using System.ComponentModel.DataAnnotations;
using BillingSystem.DTOs;
using BillingSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BillingSystem.ViewModels
{
    public class BillGenerationViewModel
    {
        [Required]
        public int PatientId { get; set; }
        
        [Display(Name = "Patient Name")]
        public string PatientName { get; set; } = string.Empty;
        
        public int Age { get; set; }
        public bool IsSenior { get; set; }

        // Search inputs
        public string? SearchMobile { get; set; }

        // Selection
        [Display(Name = "Select Services")]
        public List<int> SelectedServiceIds { get; set; } = new();
        public List<SelectListItem> AvailableServices { get; set; } = new();
        public List<SelectListItem> AvailableProviders { get; set; } = new();

        // Admissions
        public bool IsAdmitted { get; set; }
        
        [Range(0, 365)]
        [Display(Name = "Admission Days")]
        public int AdmitDays { get; set; }

        // Insurance
        public bool HasInsurance { get; set; }
        
        [Display(Name = "Insurance Provider")]
        public string? ProviderName { get; set; }
        
        [Display(Name = "Policy Number")]
        public string? PolicyNumber { get; set; }
        
        [Display(Name = "Coverage Type")]
        public InsuranceCoverageType CoverageType { get; set; }
        
        [Range(0, 100)]
        [Display(Name = "Coverage %")]
        public decimal CoveragePercent { get; set; }

        public bool ApplyInsuranceToConsultation { get; set; } = true;
        public bool ApplyInsuranceToPharmacy { get; set; } = true;
        public bool ApplyInsuranceToLabs { get; set; } = true;
        public bool ApplyInsuranceToAdmission { get; set; } = true;

        // Preview
        public BillGenerationPreviewViewModel? BillPreview { get; set; }
    }

    public class BillGenerationPreviewViewModel
    {
        public decimal ConsultationFee { get; set; }
        public decimal OptionalServicesAmount { get; set; }
        public decimal AdmitAmount { get; set; }
        public decimal GrossTotal { get; set; }
        
        public decimal Discounts { get; set; } // HighValue + Senior
        public decimal InsuranceDeduction { get; set; }
        public decimal TaxAmount { get; set; }
        
        public decimal FinalAmount { get; set; }

        public List<BillItemDto> Items { get; set; } = new();
    }
}
