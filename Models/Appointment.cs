using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("Appointment", Schema = "Healthcare")]
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [MaxLength(100)]
        public string DoctorName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Reason { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsPaid { get; set; } = false;

        // Navigation properties
        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        public virtual ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        
        public int? DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }
    }
}
