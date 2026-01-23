using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("BillItems", Schema = "Healthcare")]
    public class BillItem
    {
        [Key]
        public int BillItemId { get; set; }

        [ForeignKey("Bill")]
        public int BillId { get; set; }
        public virtual Bill? Bill { get; set; }

        public BillItemType ItemType { get; set; }
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
    }
}
