using BillingSystem.Models;

namespace BillingSystem.DTOs
{
    public class BillCalculationRequestDto
    {
        public int PatientId { get; set; }
        public List<int> SelectedServiceIds { get; set; } = new List<int>();
        public int AdmitDays { get; set; }
        public bool ApplyInsurance { get; set; }
        public InsuranceCoverageType? InsuranceType { get; set; }
        public double? InsurancePercent { get; set; }
        public List<int> SelectedLabOrderIds { get; set; } = new List<int>();
        public List<int> SelectedPrescriptionIds { get; set; } = new List<int>();
        public bool IncludeConsultationFee { get; set; } = true;
        public string? PolicyNumber { get; set; }
        public string? ProviderName { get; set; }
    }

    public class BillCalculationResultDto
    {
        public string PatientName { get; set; } = string.Empty;
        public decimal BaseConsultationFee { get; set; }
        
        public List<BillItemDto> Items { get; set; } = new List<BillItemDto>();
        public decimal OptionalServicesTotal { get; set; }
        public decimal PharmacyTotal { get; set; }
        public decimal AdmitFeeTotal { get; set; }
        
        public decimal GrossTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal SeniorDiscountAmount { get; set; }
        
        public decimal InsuranceDeduction { get; set; }
        public string InsuranceProvider { get; set; } = string.Empty;
        public double CoveragePercent { get; set; }
        
        public decimal TaxAmount { get; set; }
        public decimal FinalNetPayable { get; set; }
    }

    public class BillItemDto
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public BillItemType Type { get; set; } 
    }

    public class LabOrderListDto
    {
        public int LabOrderId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Results { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string PatientName { get; set; } = string.Empty;
    }

    public class AppointmentListDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
        public int LabOrderCount { get; set; }
    }
}
