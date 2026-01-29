using BillingSystem.Data;
using BillingSystem.Models;
using BillingSystem.Services;
using BillingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Controllers
{
    [Authorize(Roles = "Admin,Lab")]
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

        [Authorize(Roles = "Lab")]
        public async Task<IActionResult> Index(string searchString)
        {
            var labOrders = await _labOrderService.SearchLabOrdersAsync(searchString);

            ViewBag.CurrentFilter = searchString;
            return View(labOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int appointmentId, string? returnUrl = null)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null) return NotFound();

            // Fetch actual services from Billing Master
            var services = await _billingService.GetAllServicesAsync();
            
            // Rule: Duplicate Lab Order Prevention (Daily UI Level)
            // Get all tests ordered for this patient TODAY (across any appointment)
            var today = DateTime.Today;
            var existingTestNames = await _context.LabOrders
                .Where(l => l.Appointment.PatientId == appointment.PatientId && 
                            l.OrderDate.Date == today)
                .Select(l => l.TestName)
                .Distinct()
                .ToListAsync();

            // Rule: Multi-Test Visibility
            // We now show ALL active services to ensure no diagnostic is accidentally hidden.
            var availableTests = services.Where(s => s.IsActive).ToList();

            var availableTestItems = availableTests
                .Select(s => new SelectListItem 
                { 
                    Value = s.ServiceName, 
                    Text = $"[{s.ServiceCode}] {s.ServiceName}" 
                })
                .ToList();

            var model = new BulkLabOrderVM
            {
                AppointmentId = appointmentId,
                PatientName = appointment.Patient.FullName,
                AvailableTests = availableTestItems,
                ExistingTestNames = existingTestNames,
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BulkLabOrderVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _labOrderService.CreateLabOrdersAsync(model.AppointmentId, model.SelectedTestNames);

                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Details", "Appointment", new { id = model.AppointmentId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // Re-populate tests if failed
            var services = await _billingService.GetAllServicesAsync();
            model.AvailableTests = services
                .Where(s => s.IsActive)
                .Select(s => new SelectListItem { Value = s.ServiceName, Text = s.ServiceName })
                .ToList();

            return View(model);
        }

        [Authorize(Roles = "Lab")]
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(int id, string? returnUrl = null)
        {
            var labOrder = await _labOrderService.GetLabOrderByIdAsync(id);
            if (labOrder == null) return NotFound();

            var model = new LabOrderUpdateVM
            {
                LabOrderId = labOrder.LabOrderId,
                TestName = labOrder.TestName,
                Status = labOrder.Status,
                Results = labOrder.Results,
                IsPaid = labOrder.IsPaid,
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [Authorize(Roles = "Lab")]
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
            
            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            return RedirectToAction("Details", "Appointment", new { id = labOrder.AppointmentId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string? returnUrl = null)
        {
            var labOrder = await _labOrderService.GetLabOrderByIdAsync(id);
            if (labOrder == null) return NotFound();

            int appointmentId = labOrder.AppointmentId;

            try
            {
                await _labOrderService.DeleteLabOrderAsync(id);
                TempData["SuccessMessage"] = "Lab order removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Details", "Appointment", new { id = appointmentId });
        }
    }
}
