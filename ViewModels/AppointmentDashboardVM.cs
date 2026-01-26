using BillingSystem.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BillingSystem.ViewModels
{
    public class AppointmentDashboardVM
    {
        public List<AppointmentListDto> ScheduledAppointments { get; set; } = new List<AppointmentListDto>();
        public List<AppointmentListDto> CancelledAppointments { get; set; } = new List<AppointmentListDto>();
        
        public int? SelectedDoctorId { get; set; }
        public IEnumerable<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
    }
}
