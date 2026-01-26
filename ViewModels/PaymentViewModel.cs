using System.ComponentModel.DataAnnotations;
using BillingSystem.Models;

namespace BillingSystem.ViewModels
{
    public class PaymentViewModel
    {
        [Required]
        public int BillId { get; set; }

        [Required(ErrorMessage = "Payment Mode is required")]
        public PaymentMode PaymentMode { get; set; }

        public decimal PaidAmount { get; set; }

        // Logic check for <= FinalAmount will be in Controller/Service
        
        public string? TransactionReference { get; set; }
        
        // Display properties
        public decimal TotalBillAmount { get; set; }
        public decimal PendingAmount { get; set; }

        // Legacy / Compatibility Properties
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Amount to Pay")]
        public decimal AmountToPay { get; set; } // Used by existing BillingController
        public decimal TotalAmount { get; set; } // Used by existing BillingController
        public string PatientName { get; set; } = string.Empty; // Used by existing BillingController
    }
}
