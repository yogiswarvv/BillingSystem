using Microsoft.AspNetCore.Mvc;
using BillingSystem.Services;

namespace BillingSystem.Controllers
{
    // [Authorize(Roles = "Cashier,BillingStaff")]
    public class PatientLookupController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientLookupController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientById(int patientId)
        {
            var patient = await _patientService.GetPatientDetailsAsync(patientId);
            if (patient == null) return NotFound();

            return Json(new { 
                patientId = patient.PatientId,
                fullName = patient.FullName,
                age = patient.Age,
                isSenior = patient.IsSenior,
                mobileNumber = patient.MobileNumber,
                hasInsurance = patient.Insurances.Any(i => i.IsActive),
                admitDays = patient.Admissions.OrderByDescending(a => a.AdmissionId).Select(a => (int?)a.AdmitDays).FirstOrDefault() ?? 0
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
