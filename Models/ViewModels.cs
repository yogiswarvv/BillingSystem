using BillingSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.ViewModels
{
    public class PatientRegistrationVM
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        // Admission
        [Display(Name = "Admit Days (Default 0 if not admitted)")]
        [Range(0, 365)]
        public int AdmitDays { get; set; }
    }

    public class AppointmentVM
    {
        [Required]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Appointment Time")]
        public TimeSpan AppointmentTime { get; set; } = new TimeSpan(10, 0, 0);

        [MaxLength(500)]
        public string? Reason { get; set; }

        public IEnumerable<SelectListItem>? Patients { get; set; }
        public IEnumerable<SelectListItem>? Doctors { get; set; }
    }

    public class LabOrderUpdateVM
    {
        public int LabOrderId { get; set; }
        public string TestName { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string? Results { get; set; }

        [Required]
        public string Status { get; set; } = "Completed";
        
        public bool IsPaid { get; set; }
    }

    public class BulkLabOrderVM
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please select at least one test.")]
        public List<string> SelectedTestNames { get; set; } = new List<string>();
        
        public List<SelectListItem> AvailableTests { get; set; } = new List<SelectListItem>();
    }

    public class AppointmentDetailsVM
    {
        public Appointment Appointment { get; set; } = null!;
        public List<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
    }
}
