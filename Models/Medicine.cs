using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("Medicines", Schema = "Healthcare")]
    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? DosageStrength { get; set; } // e.g., 500mg, 5ml

        [MaxLength(100)]
        public string? Category { get; set; } // e.g., Antibiotic, Analgesic

        [Range(0.01, 1000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerUnit { get; set; }

        public int Stock { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
