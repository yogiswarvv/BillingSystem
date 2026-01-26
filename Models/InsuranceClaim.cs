using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("InsuranceClaims", Schema = "Healthcare")]
    public class InsuranceClaim
    {
        [Key]
        public int ClaimID { get; set; }

        [ForeignKey("Bill")]
        public int BillID { get; set; }
        public virtual Bill? Bill { get; set; }

        [Required]
        [MaxLength(50)]
        public string PolicyNumber { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ClaimAmountRequested { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ApprovedAmount { get; set; } = 0.00m;

        [MaxLength(50)]
        public string? ApprovalCode { get; set; }

        [MaxLength(50)]
        public string ClaimStatus { get; set; } = "Pending"; // Pending, Approved, Rejected, Settled

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        public DateTime? ProcessedDate { get; set; }
    }
}
