using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("InsuranceMemberRegistry", Schema = "Healthcare")]
    public class InsuranceMemberRegistry
    {
        [Key]
        public int MemberID { get; set; }

        [Required]
        [MaxLength(50)]
        public string PolicyNumber { get; set; } = string.Empty;

        [ForeignKey("Provider")]
        public int ProviderID { get; set; }
        public virtual InsuranceProvider? Provider { get; set; }

        [ForeignKey("Plan")]
        public int PlanID { get; set; }
        public virtual InsurancePlan? Plan { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active, Expired, Suspended

        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingBalance { get; set; }

        public DateTime? LastRenewalDate { get; set; }
    }
}
