using BillingSystem.Models;
using System.Collections.Generic;

namespace BillingSystem.ViewModels
{
    public class PatientProfileVM
    {
        public Patient Patient { get; set; } = null!;
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public Admission? ActiveAdmission { get; set; }
        public List<Admission> AdmissionHistory { get; set; } = new List<Admission>();
        public List<Bill> BillHistory { get; set; } = new List<Bill>();
        
        // Stats
        public decimal TotalUnpaidAmount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal PendingCharges { get; set; }
        public decimal TotalOutstanding => TotalUnpaidAmount + PendingCharges;
        public int TotalVisits { get; set; }
        public bool IsAdmitted => ActiveAdmission != null;
    }
}
