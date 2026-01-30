using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BillingSystem.ViewModels
{
    public class PatientDashboardVM
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string AgeGender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        
        // Appointment Info for "Upcoming"
        public string NextAppointmentDisplay { get; set; } = "No Appointment";
        public string AppointmentStatus { get; set; } = "N/A";
        public int? AppointmentId { get; set; }
        public bool HasUpcomingAppointment { get; set; }
        
        // Statistics
        public int PendingLabOrders { get; set; }
        public int RecentPrescriptions { get; set; }
        public bool IsActive { get; set; }
        public string StatusClass => IsActive ? "bg-success" : "bg-danger";
    }

    public class PatientDashboardListVM
    {
        public List<PatientDashboardVM> Patients { get; set; } = new();
        public int TotalPatientsCount { get; set; }
        public int UpcomingAppointmentsCount { get; set; }
    }
}
