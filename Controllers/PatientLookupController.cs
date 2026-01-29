using Microsoft.AspNetCore.Mvc;
using BillingSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Controllers
{
    // [Authorize(Roles = "Cashier,BillingStaff")]
    public class PatientLookupController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly AppointmentService _appointmentService;
        private readonly LabOrderService _labOrderService;
        private readonly IInsuranceService _insuranceService;
        private readonly BillingSystem.Data.HospitalDbContext _context;
        private readonly BillingSystem.Repositories.IUnitOfWork _unitOfWork;
 
        public PatientLookupController(IPatientService patientService, AppointmentService appointmentService, LabOrderService labOrderService, IInsuranceService insuranceService, BillingSystem.Data.HospitalDbContext context, BillingSystem.Repositories.IUnitOfWork unitOfWork)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _labOrderService = labOrderService;
            _insuranceService = insuranceService;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientById(int patientId)
        {
            var patient = await _patientService.GetPatientDetailsAsync(patientId);
            if (patient == null) return NotFound();

            var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            var pendingTests = await _labOrderService.GetAllLabOrdersAsync(); 
            var pendingMedicines = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(_context.Prescriptions
                .Include(p => p.Medicine)
                .Where(p => p.Appointment.PatientId == patientId && (p.Status == "Suggested" || p.Status == "Purchased") && !p.IsPaid));

            return Json(new { 
                patientId = patient.PatientId,
                fullName = patient.FullName,
                age = patient.Age,
                isSenior = patient.IsSenior,
                mobileNumber = patient.MobileNumber,
                hasInsurance = patient.Insurances.Any(i => i.IsActive),
                admitDays = patient.Admissions.OrderByDescending(a => a.AdmissionId).Select(a => (int?)a.AdmitDays).FirstOrDefault() ?? 0,
                pendingAppointments = appointments.Where(a => a.Status == "Scheduled").Select(a => new { a.AppointmentId, a.Reason, a.AppointmentDate, a.DoctorName }),
                pendingTests = pendingTests.Where(l => l.Appointment.PatientId == patientId && !l.IsPaid).Select(l => new { l.LabOrderId, l.TestName, l.OrderDate }),
                pendingMedicines = pendingMedicines.Select(p => new { p.PrescriptionId, p.Medicine.Name, p.ActualQuantity, p.Medicine.PricePerUnit })
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

        [HttpGet]
        public async Task<IActionResult> GetInsuranceDetails(int patientId, string provider, string policyNumber)
        {
            if (string.IsNullOrEmpty(policyNumber))
                return BadRequest("Policy Number is required");

            // 1. Try Global SP Check (Local Database)
            var localPolicy = await _unitOfWork.Bills.CheckPatientInsuranceSPAsync(patientId, provider?.Trim() ?? "", policyNumber.Trim());
            
            if (localPolicy != null)
            {
                return Json(new
                {
                    coveragePercent = localPolicy.CoveragePercent,
                    planName = "Direct Coverage", // Or fetch plan if available
                    remainingBalance = 1000000 // Infinite for direct lookup unless we have specific ledger
                });
            }

            // 2. Fallback to External Registry Simulation
            var member = await _insuranceService.GetMemberDetailsAsync(policyNumber);
            if (member == null)
                return NotFound("please enter correct details or please contact the admin");

            // Fetch Plan to get details
            var plan = await _context.InsurancePlans.FirstOrDefaultAsync(p => p.PlanID == member.PlanID);

            return Json(new
            {
                coveragePercent = plan?.CoveragePercentage ?? 0,
                planName = plan?.PlanName ?? "Unknown",
                remainingBalance = member.RemainingBalance
            });
        }
    }
}
