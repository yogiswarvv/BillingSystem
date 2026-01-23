using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("Bills", Schema = "Healthcare")]
    public class Bill
    {
        [Key]
        public int BillId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        public DateTime BillDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ConsultationFee { get; set; } = 500m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal OptionalServicesAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdmitAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Gross

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SeniorDiscount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InsuranceDeduction { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalAmount { get; set; }

        public BillStatus Status { get; set; } = BillStatus.Unpaid;

        public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
