using BillingSystem.Models;
using BillingSystem.Services;
using BillingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BillingSystem.Controllers
{
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

        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAppointmentListAsync();
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? patientId)
        {
            var patients = await _patientService.GetAllPatientsAsync();
            var doctors = await _doctorService.GetAvailableDoctorsAsync();

            var model = new AppointmentVM
            {
                PatientId = patientId ?? 0,
                Patients = patients.Select(p => new SelectListItem { Value = p.PatientId.ToString(), Text = p.FullName }),
                Doctors = doctors.Select(d => new SelectListItem { Value = d.DoctorId.ToString(), Text = $"{d.FirstName} {d.LastName} ({d.Specialization})" })
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

            var labOrders = await _appointmentService.GetLabOrdersByAppointmentIdAsync(id);

            var model = new AppointmentDetailsVM
            {
                Appointment = appointment,
                LabOrders = labOrders
            };

            return View(model);
        }
    }
}
