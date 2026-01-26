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
        [MinLength(3, ErrorMessage = "First name must be at least 3 characters long.")]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name cannot contain numbers.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(3, ErrorMessage = "Last name must be at least 3 characters long.")]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name cannot contain numbers.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [PastDate(ErrorMessage = "Date of birth must be a past date.")]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [NotMapped]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public Gender Gender { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
        public string MobileNumber { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public string? Address { get; set; }

        [NotMapped]
        public bool IsSenior => Age >= 60;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; }

        // Navigation
        public virtual ICollection<PatientService> PatientServices { get; set; } = new List<PatientService>();
        public virtual ICollection<Admission> Admissions { get; set; } = new List<Admission>();
        public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
