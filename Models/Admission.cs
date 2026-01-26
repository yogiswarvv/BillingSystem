using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("Admissions", Schema = "Healthcare")]
    public class Admission
    {
        [Key]
        public int AdmissionId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        public DateTime AdmitDate { get; set; }
        public DateTime? DischargeDate { get; set; }

        public int AdmitDays { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FeePerDay { get; set; } = 2000m;
        public bool IsPaid { get; set; } = false;
    }
}
