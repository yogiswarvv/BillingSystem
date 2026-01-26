using System.Diagnostics;
using BillingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Data.HospitalDbContext _context;

        public HomeController(ILogger<HomeController> logger, Data.HospitalDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            
            var model = new ViewModels.HomeDashboardVM
            {
                TotalPatients = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(_context.Patients.Where(p => p.IsActive)),
                AppointmentsToday = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(_context.Appointments.Where(a => a.AppointmentDate.Date == today)),
                TotalRevenue = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync(_context.Bills.Where(b => b.Status == BillStatus.Paid), b => (decimal?)b.FinalAmount) ?? 0,
                PendingLabOrders = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(_context.LabOrders.Where(l => l.Status == "Pending")),
                
                NewPatientsToday = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(_context.Patients.Where(p => p.CreatedDate.Date == today)),
                TodaysRevenue = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync(_context.Bills.Where(b => b.Status == BillStatus.Paid && b.BillDate.Date == today), b => (decimal?)b.FinalAmount) ?? 0,
                CompletedAppointmentsToday = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(_context.Appointments.Where(a => a.AppointmentDate.Date == today && a.Status == "Completed"))
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
