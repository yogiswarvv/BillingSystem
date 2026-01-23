using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("Insurances", Schema = "Healthcare")]
    public class Insurance
    {
        [Key]
        public int InsuranceId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        public string ProviderName { get; set; } = string.Empty;
        public string? PolicyNumber { get; set; }

        [Range(0, 100)]
        public double CoveragePercent { get; set; }

        public InsuranceCoverageType CoverageType { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
