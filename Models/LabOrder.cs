using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("LabOrder", Schema = "Healthcare")]
    public class LabOrder
    {
        [Key]
        public int LabOrderId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an appointment.")]
        public int AppointmentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string TestName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Cost { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [MaxLength(1000)]
        public string? Results { get; set; }

        public DateTime? CompletedDate { get; set; }
        public bool IsPaid { get; set; } = false;

        // Navigation property
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; } = null!;
    }
}
