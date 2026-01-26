using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("Prescriptions", Schema = "Healthcare")]
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        [Required]
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; } = null!;

        [Required]
        public int MedicineId { get; set; }
        public virtual Medicine Medicine { get; set; } = null!;

        public int SuggestedQuantity { get; set; } // Doctor's suggestion
        public int ActualQuantity { get; set; }    // Patient's choice (what they actually buy)

        [Required]
        public string Status { get; set; } = "Suggested"; // Suggested, Purchased, Cancelled

        public bool IsPaid { get; set; }
        
        public DateTime PrescribedDate { get; set; } = DateTime.Now;
    }
}
