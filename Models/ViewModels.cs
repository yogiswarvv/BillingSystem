using BillingSystem.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BillingSystem.ViewModels
{
    public class PatientRegistrationVM
    {
        [Required]
        [MinLength(3, ErrorMessage = "First name must be at least 3 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name cannot contain numbers.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(3, ErrorMessage = "Last name must be at least 3 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name cannot contain numbers.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [PastDate(ErrorMessage = "Date of birth must be a past date.")]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required]
        public Gender Gender { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
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

        public string? ReturnUrl { get; set; }

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
        public string? ReturnUrl { get; set; }
    }

    public class BulkLabOrderVM
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please select at least one test.")]
        public List<string> SelectedTestNames { get; set; } = new List<string>();
        
        public List<SelectListItem> AvailableTests { get; set; } = new List<SelectListItem>();
        public List<string> ExistingTestNames { get; set; } = new List<string>();
        public string? ReturnUrl { get; set; }
    }

    public class AppointmentDetailsVM
    {
        public Appointment Appointment { get; set; } = null!;
        public List<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }

    public class PrescriptionCreateVM
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please select a medicine.")]
        public int MedicineId { get; set; }
        public List<SelectListItem> AvailableMedicines { get; set; } = new List<SelectListItem>();
        
        [Range(1, 100)]
        public int SuggestedQuantity { get; set; }
    }

    public class BulkPrescriptionVM
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public List<MedicineSelectionVM> Medicines { get; set; } = new List<MedicineSelectionVM>();
    }

    public class MedicineSelectionVM
    {
        public int MedicineId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class PrescriptionUpdateVM
    {
        public int PrescriptionId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public int SuggestedQuantity { get; set; }
        
        [Range(0, 100, ErrorMessage = "Quantity must be between 0 and 100.")]
        public int ActualQuantity { get; set; }
        
        public string Status { get; set; } = "Purchased";
    }
}
