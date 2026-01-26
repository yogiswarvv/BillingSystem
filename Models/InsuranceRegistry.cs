using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    // Base class to ensure consistency across separate provider registries
    public abstract class BaseInsuranceRegistry
    {
        [Key]
        public int MemberID { get; set; }

        [Required]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public int PlanID { get; set; }

        [Required]
        public decimal RemainingBalance { get; set; }

        public string Status { get; set; } = "Active";
    }

    [Table("ApolloMunich_Registry", Schema = "Healthcare")]
    public class ApolloMunichRegistry : BaseInsuranceRegistry { }

    [Table("HDFCErgo_Registry", Schema = "Healthcare")]
    public class HDFCErgoRegistry : BaseInsuranceRegistry { }

    [Table("StarHealth_Registry", Schema = "Healthcare")]
    public class StarHealthRegistry : BaseInsuranceRegistry { }

    // Unified view for Service Layer
    public class FederatedMemberDetails
    {
        public int MemberID { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int PlanID { get; set; }
        public decimal RemainingBalance { get; set; }
        public string Status { get; set; } = "Active";
        public string ProviderName { get; set; } = string.Empty;
    }
}
