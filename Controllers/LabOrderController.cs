using BillingSystem.Data;
using BillingSystem.Models;
using BillingSystem.Services;
using BillingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Controllers
{
    public class LabOrderController : Controller
    {
        private readonly LabOrderService _labOrderService;
        private readonly AppointmentService _appointmentService;
        private readonly IBillingService _billingService;
        private readonly HospitalDbContext _context;

        public LabOrderController(
            LabOrderService labOrderService, 
            AppointmentService appointmentService, 
            IBillingService billingService,
            HospitalDbContext context)
        {
            _labOrderService = labOrderService;
            _appointmentService = appointmentService;
            _billingService = billingService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var labOrders = await _labOrderService.GetAllLabOrdersAsync();
            return View(labOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int appointmentId)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null) return NotFound();

            // Fetch actual services from Billing Master
            var services = await _billingService.GetAllServicesAsync();
            
            // Filter for Lab/Diagnostic related departments (Pathology, Radiology)
            var availableTests = services
                .Where(s => s.IsActive && (s.Department == "Pathology" || s.Department == "Radiology" || s.Department == "Diagnostics" || s.Department == "Cardiology"))
                .Select(s => new SelectListItem 
                { 
                    Value = s.ServiceName, 
                    Text = $"[{s.ServiceCode}] {s.ServiceName} ({s.Cost:C})" 
                })
                .ToList();

            // Fallback if no specific departments found - show all active services
            if (!availableTests.Any())
            {
                availableTests = services
                    .Where(s => s.IsActive)
                    .Select(s => new SelectListItem { Value = s.ServiceName, Text = s.ServiceName })
                    .ToList();
            }

            var model = new BulkLabOrderVM
            {
                AppointmentId = appointmentId,
                PatientName = appointment.Patient.FullName,
                AvailableTests = availableTests
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BulkLabOrderVM model)
        {
            if (ModelState.IsValid)
            {
                foreach (var testName in model.SelectedTestNames)
                {
                    var labOrder = new LabOrder
                    {
                        AppointmentId = model.AppointmentId,
                        TestName = testName,
                        Status = "Pending",
                        IsPaid = false
                    };
                    await _labOrderService.CreateLabOrderAsync(labOrder);
                }
                return RedirectToAction("Details", "Appointment", new { id = model.AppointmentId });
            }

            // Re-populate tests if failed
            var services = await _billingService.GetAllServicesAsync();
            model.AvailableTests = services
                .Where(s => s.IsActive)
                .Select(s => new SelectListItem { Value = s.ServiceName, Text = s.ServiceName })
                .ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var labOrder = await _labOrderService.GetLabOrderByIdAsync(id);
            if (labOrder == null) return NotFound();

            var model = new LabOrderUpdateVM
            {
                LabOrderId = labOrder.LabOrderId,
                TestName = labOrder.TestName,
                Status = labOrder.Status,
                Results = labOrder.Results,
                IsPaid = labOrder.IsPaid
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(LabOrderUpdateVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var labOrder = await _labOrderService.GetLabOrderByIdAsync(model.LabOrderId);
            if (labOrder == null) return NotFound();

            // OPD Check: If person not admitted, check for payment
            var patientId = labOrder.Appointment.PatientId;
            var isActiveAdmission = await _context.Admissions
                .AnyAsync(a => a.PatientId == patientId && a.DischargeDate == null);

            if (!isActiveAdmission && !labOrder.IsPaid)
            {
                ModelState.AddModelError("", "This patient is an outpatient. Lab results can only be finalized after payment is received at the Billing Desk.");
                return View(model);
            }

            await _labOrderService.UpdateLabOrderStatusAsync(model.LabOrderId, model.Status, model.Results);
            
            return RedirectToAction("Details", "Appointment", new { id = labOrder.AppointmentId });
        }
    }
}
