using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("PastRecordPatients", Schema = "Healthcare")]
    public class PastRecordPatient
    {
        [Key]
        public int PastPatientId { get; set; }

        public int OriginalPatientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        [Required]
        [MaxLength(10)]
        public string MobileNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ArchivedDate { get; set; } = DateTime.Now;

        public string? ArchivedBy { get; set; }
    }
}
