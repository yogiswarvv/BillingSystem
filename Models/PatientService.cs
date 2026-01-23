using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillingSystem.Models
{
    [Table("PatientServices", Schema = "Healthcare")]
    public class PatientService
    {
        [Key]
        public int PatientServiceId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [ForeignKey("ServiceMaster")]
        public int ServiceId { get; set; }
        public virtual ServiceMaster? ServiceMaster { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime ServiceDate { get; set; } = DateTime.Now;
    }
}
