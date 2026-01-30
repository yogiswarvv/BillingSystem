using BillingSystem.Models;
using BillingSystem.Services;
using BillingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BillingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly DoctorService _doctorService;

        public AppointmentController(
            AppointmentService appointmentService,
            IPatientService patientService,
            DoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        public async Task<IActionResult> Index(int? doctorId)
        {
            var allAppointments = await _appointmentService.GetAppointmentListAsync(doctorId);
            var doctors = await _doctorService.GetAvailableDoctorsAsync();

            var now = DateTime.Now;

            // 1. AUTO-UPDATE: Mark past "Scheduled" appointments as "Completed"
            // If the appointment time has passed and they haven't been cancelled/processed, we assume they are completed.
            var expiredAppointments = allAppointments
                .Where(a => a.Status == "Scheduled" && 
                           (a.AppointmentDate.Date < now.Date || (a.AppointmentDate.Date == now.Date && a.AppointmentTime <= now.TimeOfDay)))
                .ToList();

            if (expiredAppointments.Any())
            {
                // Fix: expiredAppointments is a DTO list. We need to fetch entities to update them via Service.
                foreach (var apptDto in expiredAppointments)
                {
                    var apptEntity = await _appointmentService.GetAppointmentByIdAsync(apptDto.AppointmentId);
                    if (apptEntity != null)
                    {
                        await _appointmentService.UpdateAppointmentStatusAsync(apptEntity.AppointmentId, "Completed");
                    }
                }
            }

            var model = new AppointmentDashboardVM
            {
                ScheduledAppointments = allAppointments
                    .Where(a => a.Status == "Scheduled" && 
                               (a.AppointmentDate.Date > now.Date || (a.AppointmentDate.Date == now.Date && a.AppointmentTime > now.TimeOfDay)))
                    .OrderBy(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime)
                    .ToList(),

                CompletedAppointments = allAppointments
                    .Where(a => a.Status != "Cancelled" && 
                               (a.AppointmentDate.Date < now.Date || (a.AppointmentDate.Date == now.Date && a.AppointmentTime <= now.TimeOfDay)))
                    .OrderByDescending(a => a.AppointmentDate).ThenByDescending(a => a.AppointmentTime)
                    .ToList(),

                CancelledAppointments = allAppointments
                    .Where(a => a.Status == "Cancelled")
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToList(),
                
                SelectedDoctorId = doctorId,
                Doctors = doctors.Select(d => new SelectListItem 
                { 
                    Value = d.DoctorId.ToString(), 
                    Text = $"{d.FirstName} {d.LastName} ({d.Specialization})",
                    Selected = d.DoctorId == doctorId
                })
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? patientId, string? returnUrl = null)
        {
            var patients = await _patientService.GetAllPatientsAsync();
            var doctors = await _doctorService.GetAvailableDoctorsAsync();

            var model = new AppointmentVM
            {
                PatientId = patientId ?? 0,
                Patients = patients.Select(p => new SelectListItem 
                { 
                    Value = p.PatientId.ToString(), 
                    Text = p.FullName 
                }),
                Doctors = doctors.Select(d => new SelectListItem { Value = d.DoctorId.ToString(), Text = $"{d.FirstName} {d.LastName} ({d.Specialization})" }),
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AppointmentVM model)
        {
            if (ModelState.IsValid)
            {
                var doctor = (await _doctorService.GetAvailableDoctorsAsync()).FirstOrDefault(d => d.DoctorId == model.DoctorId);
                
                var appointment = new Appointment
                {
                    PatientId = model.PatientId,
                    DoctorId = model.DoctorId,
                    DoctorName = doctor != null ? $"{doctor.FirstName} {doctor.LastName}" : "Unknown",
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTime = model.AppointmentTime,
                    Reason = model.Reason,
                    Status = "Scheduled"
                };

                await _appointmentService.CreateAppointmentAsync(appointment);
                
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction(nameof(Index));
            }

            // Re-populate lists if failed
            var patients = await _patientService.GetAllPatientsAsync();
            var doctors = await _doctorService.GetAvailableDoctorsAsync();
            model.Patients = patients.Select(p => new SelectListItem { Value = p.PatientId.ToString(), Text = p.FullName });
            model.Doctors = doctors.Select(d => new SelectListItem { Value = d.DoctorId.ToString(), Text = $"{d.FirstName} {d.LastName} ({d.Specialization})" });

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();

            var model = new AppointmentDetailsVM
            {
                Appointment = appointment,
                LabOrders = appointment.LabOrders.ToList(),
                Prescriptions = appointment.Prescriptions.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, DateTime date)
        {
            var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, date);
            var formattedSlots = slots.Select(s => new {
                time = s.ToString(@"hh\:mm"),
                display = DateTime.Today.Add(s).ToString("hh:mm tt")
            });
            return Json(formattedSlots);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id, string? returnUrl = null)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment != null)
            {
                // Business Rule: Cannot cancel if there are pending or unpaid lab orders
                if (appointment.LabOrders.Any(l => l.Status == "Pending" || !l.IsPaid))
                {
                    TempData["ErrorMessage"] = "Cannot cancel appointment: There are pending or unpaid lab orders associated with it.";
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(Index));
                }

                await _appointmentService.UpdateAppointmentStatusAsync(id, "Cancelled");
                TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, string? returnUrl = null)
        {
            return await Cancel(id, returnUrl);
        }
    }
}
