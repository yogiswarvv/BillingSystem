using System;

namespace BillingSystem.ViewModels
{
    public class HomeDashboardVM
    {
        public int TotalPatients { get; set; }
        public int AppointmentsToday { get; set; }
        public decimal TotalRevenue { get; set; }
        public int PendingLabOrders { get; set; }
        
        // Trends / Secondary Stats
        public int NewPatientsToday { get; set; }
        public decimal TodaysRevenue { get; set; }
        public int CompletedAppointmentsToday { get; set; }
    }
}
