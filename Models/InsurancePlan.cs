using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("InsurancePlans", Schema = "Healthcare")]
    public class InsurancePlan
    {
        [Key]
        public int PlanID { get; set; }

        [ForeignKey("Provider")]
        public int ProviderID { get; set; }
        public virtual InsuranceProvider? Provider { get; set; }

        [Required]
        [MaxLength(100)]
        public string PlanName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5,2)")]
        public decimal CoveragePercentage { get; set; } = 100.00m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal CoPayAmount { get; set; } = 0.00m;

        public bool RequiresPreAuth { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxBenefitPerClaim { get; set; }
    }
}
