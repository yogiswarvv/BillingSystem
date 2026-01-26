using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("InsuranceProviders", Schema = "Healthcare")]
    public class InsuranceProvider
    {
        [Key]
        public int ProviderID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProviderName { get; set; } = string.Empty;

        public bool IsLinked { get; set; } = true;

        public DateTime? AgreementExpiryDate { get; set; }

        [MaxLength(100)]
        public string? ContactEmail { get; set; }

        [MaxLength(20)]
        public string? TollFreeNumber { get; set; }

        public virtual ICollection<InsurancePlan> Plans { get; set; } = new List<InsurancePlan>();
    }
}
