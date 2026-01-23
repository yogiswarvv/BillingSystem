using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("Payments", Schema = "Healthcare")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("Bill")]
        public int BillId { get; set; }
        public virtual Bill? Bill { get; set; }

        public PaymentMode PaymentMode { get; set; }
        public string? TransactionRef { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; }

        public DateTime PaidDate { get; set; } = DateTime.Now;
    }
}
