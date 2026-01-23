using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("Patients", Schema = "Healthcare")]
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [MinLength(3)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only alphabets.")]
        public string FullName { get; set; } = string.Empty;

        [Range(0, 120)]
        public int Age { get; set; }

        public Gender Gender { get; set; }

        [Required]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Mobile number must be 10 digits and start with 6-9.")]
        public string MobileNumber { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public bool IsSenior { get; set; } // Computed

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }

        // Navigation
        public virtual ICollection<PatientService> PatientServices { get; set; } = new List<PatientService>();
        public virtual ICollection<Admission> Admissions { get; set; } = new List<Admission>();
        public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
    }
}
