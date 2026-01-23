using Microsoft.AspNetCore.Mvc;
using BillingSystem.Services;

namespace BillingSystem.Controllers
{
    // [Authorize(Roles = "Cashier,BillingStaff")]
    public class PatientLookupController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly AppointmentService _appointmentService;
        private readonly LabOrderService _labOrderService;

        public PatientLookupController(IPatientService patientService, AppointmentService appointmentService, LabOrderService labOrderService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _labOrderService = labOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientById(int patientId)
        {
            var patient = await _patientService.GetPatientDetailsAsync(patientId);
            if (patient == null) return NotFound();

            var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            var pendingTests = await _labOrderService.GetAllLabOrdersAsync(); // We will filter by patient via appointment

            return Json(new { 
                patientId = patient.PatientId,
                fullName = patient.FullName,
                age = patient.Age,
                isSenior = patient.IsSenior,
                mobileNumber = patient.MobileNumber,
                hasInsurance = patient.Insurances.Any(i => i.IsActive),
                admitDays = patient.Admissions.OrderByDescending(a => a.AdmissionId).Select(a => (int?)a.AdmitDays).FirstOrDefault() ?? 0,
                pendingAppointments = appointments.Where(a => a.Status == "Scheduled").Select(a => new { a.AppointmentId, a.Reason, a.AppointmentDate, a.DoctorName }),
                pendingTests = pendingTests.Where(l => l.Appointment.PatientId == patientId && l.Status == "Pending").Select(l => new { l.LabOrderId, l.TestName, l.OrderDate })
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientByMobile(string mobileNumber)
        {
            var patient = await _patientService.GetPatientByMobileAsync(mobileNumber);
            if (patient == null) return NotFound();

            return Json(new
            {
                patientId = patient.PatientId,
                fullName = patient.FullName,
                age = patient.Age,
                isSenior = patient.IsSenior,
                hasInsurance = patient.Insurances.Any(i => i.IsActive),
                admitDays = patient.Admissions.OrderByDescending(a => a.AdmissionId).Select(a => (int?)a.AdmitDays).FirstOrDefault() ?? 0
            });
        }
    }
}
