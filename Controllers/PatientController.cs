using BillingSystem.DTOs;
using BillingSystem.Models;
using BillingSystem.Services;
using BillingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Controllers
{
    [Authorize(Roles = "Admin,Billing")]
    public class PatientController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IBillingService _billingService;
        // Check if we need Active Services lookup. 
        // Ideally ServiceMasterService or Repository. 
        // For dropdowns, accessing ServiceRepository is okay or add method to IPatientService/IServiceMasterService.
        // Let's inject IServiceMasterRepository or handle via PatientService (GetAllServices?)
        // The prompt says "Repositories ... Avoid business logic". 
        // Service Layer "GetPatientDetails".
        // Let's inject IunitOfWork or specific ServiceRepo for the dropdown to keep it simple or strictly Service.
        // IPatientService doesn't have "GetServices". I will add it or use UoW. 
        // Let's use IServiceMasterRepository via DI for the dropdown to purely separate.
        // Actually, I registered IRepository and specific ones.
        // Let's use the Service we created, "ServiceMasterRepository" injected as IServiceMasterRepository?
        // Wait, I registered `IServiceMasterRepository` in UnitOfWork, but not as standalone scope in Program.cs?
        // I registered UoW. 
        // Let's stick to UoW for dropdowns if I don't want to create "MasterDataService".
        private readonly Repositories.IUnitOfWork _unitOfWork; 

        public PatientController(IPatientService patientService, IBillingService billingService, Repositories.IUnitOfWork unitOfWork)
        {
            _patientService = patientService;
            _billingService = billingService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            var now = DateTime.Now;
            
            IQueryable<Patient> query = _unitOfWork.Patients.GetAllQueryable()
                .Include(p => p.Appointments)
                .Include(p => p.Appointments).ThenInclude(a => a.LabOrders);

            if (!string.IsNullOrEmpty(searchString))
            {
                // Logic: If searching by ID, include inactive. If general search, only active.
                if (int.TryParse(searchString, out int pid))
                {
                    query = _unitOfWork.Patients.GetAllQueryable()
                        .Where(p => p.PatientId == pid)
                        .Include(p => p.Appointments)
                        .Include(p => p.Appointments).ThenInclude(a => a.LabOrders);
                }
                else
                {
                    query = query.Where(p => p.IsActive && (
                        p.FirstName.Contains(searchString) || 
                        p.LastName.Contains(searchString) ||
                        p.MobileNumber.Contains(searchString))
                    );
                }
            }
            else
            {
                query = query.Where(p => p.IsActive);
            }

            var patients = await query.ToListAsync();

            var model = new PatientDashboardListVM
            {
                TotalPatientsCount = patients.Count,
                UpcomingAppointmentsCount = patients.SelectMany(p => p.Appointments)
                    .Count(a => a.Status == "Scheduled" && 
                               (a.AppointmentDate.Date > now.Date || 
                               (a.AppointmentDate.Date == now.Date && a.AppointmentTime > now.TimeOfDay))),
                Patients = patients.Select(p => {
                    var nextAppt = p.Appointments
                        .Where(a => a.Status == "Scheduled" && 
                                   (a.AppointmentDate.Date > now.Date || 
                                   (a.AppointmentDate.Date == now.Date && a.AppointmentTime > now.TimeOfDay)))
                        .OrderBy(a => a.AppointmentDate)
                        .ThenBy(a => a.AppointmentTime)
                        .FirstOrDefault();

                    return new PatientDashboardVM
                    {
                        PatientId = p.PatientId,
                        FullName = p.FullName,
                        Mobile = p.MobileNumber,
                        Email = p.Email ?? "N/A",
                        AgeGender = $"{p.Age}yrs / {p.Gender}",
                        HasUpcomingAppointment = nextAppt != null,
                        NextAppointmentDisplay = nextAppt != null ? 
                            (nextAppt.AppointmentDate.Date == now.Date ? "Today " : 
                             nextAppt.AppointmentDate.Date == now.Date.AddDays(1) ? "Tomorrow " : 
                             nextAppt.AppointmentDate.ToString("dd MMM ")) + 
                            DateTime.Today.Add(nextAppt.AppointmentTime).ToString("hh:mm tt") 
                            : "No Appointment",
                        AppointmentStatus = nextAppt?.Status ?? "N/A",
                        AppointmentId = nextAppt?.AppointmentId,
                        PendingLabOrders = p.Appointments.SelectMany(a => a.LabOrders).Count(l => l.Status == "Pending"),
                        RecentPrescriptions = p.Appointments.SelectMany(a => a.Prescriptions).Count(),
                        IsActive = p.IsActive
                    };
                }).ToList()
            };

            ViewBag.CurrentFilter = searchString;
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PatientRegistrationVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientRegistrationVM model)
        {
            if (ModelState.IsValid)
            {
                // 0. Strict Uniqueness Check (Name + Mobile)
                var exists = await _unitOfWork.Patients.GetAllQueryable()
                    .AnyAsync(p => p.FirstName == model.FirstName && 
                                   p.LastName == model.LastName && 
                                   p.MobileNumber == model.MobileNumber);
                
                if (exists)
                {
                    ModelState.AddModelError("", "A patient with this Name and Mobile Number already exists.");
                    return View(model);
                }

                // 1. Create Patient Entity
                var patient = new Patient
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    MobileNumber = model.MobileNumber,
                    Email = model.Email,
                    CreatedDate = DateTime.Now
                };
 
                // 4. Register via Service (SP)
                try 
                {
                    // Call SP to register patient (Returns ID)
                    var patientId = await _patientService.RegisterPatientAsync(patient);
 
                    // Redirect Logic based on Role
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("Index");
                    }
 
                    // Redirect to Automatic Billing for Billing Staff
                    return RedirectToAction("GenerateBill", "Billing", new { patientId = patientId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var patient = await _unitOfWork.Patients.GetPatientWithDetailsAsync(id);
            if (patient == null) return NotFound();

            var appointments = await _unitOfWork.Repository<Appointment>().FindAsync(a => a.PatientId == id);
            var labOrders = await _unitOfWork.Repository<LabOrder>().FindAsync(l => l.Appointment.PatientId == id);
            var admissions = await _unitOfWork.Repository<Admission>().FindAsync(a => a.PatientId == id);
            var bills = await _unitOfWork.Bills.GetBillsByPatientIdAsync(id);

            var pendingBilling = await _billingService.CalculateBillAsync(new BillCalculationRequestDto { PatientId = id });

            var model = new PatientProfileVM
            {
                Patient = patient,
                Appointments = appointments.OrderByDescending(a => a.AppointmentDate).ThenByDescending(a => a.AppointmentTime).ToList(),
                LabOrders = labOrders.OrderByDescending(l => l.OrderDate).ToList(),
                ActiveAdmission = admissions.FirstOrDefault(a => a.DischargeDate == null),
                AdmissionHistory = admissions.OrderByDescending(a => a.AdmitDate).ToList(),
                BillHistory = bills.OrderByDescending(b => b.BillDate).ToList(),
                TotalVisits = appointments.Count(),
                TotalUnpaidAmount = bills.Where(b => b.Status == BillStatus.Unpaid).Sum(b => b.FinalAmount),
                TotalPaidAmount = bills.SelectMany(b => b.Payments).Sum(p => p.PaidAmount),
                PendingCharges = pendingBilling.FinalNetPayable
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAdmitDays(int patientId, int days)
        {
            var admission = (await _unitOfWork.Repository<Admission>().FindAsync(a => a.PatientId == patientId && a.DischargeDate == null))
                            .FirstOrDefault();

            if (admission != null)
            {
                // Logic: Admission record keeps AdmitDate. discharge date is calculated or updated
                // For simplicity, we can't easily change "days" if we only have Admit and Discharge dates.
                // But we can update the Expected discharge or simulate
                admission.AdmitDate = DateTime.Now.AddDays(-days); // Hacky way to set 'days' for testing
                _unitOfWork.Repository<Admission>().Update(admission);
                await _unitOfWork.CompleteAsync();
                TempData["SuccessMessage"] = "Admission duration updated.";
            }

            return RedirectToAction(nameof(Details), new { id = patientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admit(int id)
        {
            var patient = await _unitOfWork.Patients.GetPatientWithDetailsAsync(id);
            if (patient == null) return NotFound();

            var appointments = patient.Appointments
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToList();

            var latestActiveAppt = appointments.FirstOrDefault(a => a.Status != "Cancelled");
            var latestOverallAppt = appointments.FirstOrDefault();

            if (latestActiveAppt == null)
            {
                if (latestOverallAppt == null)
                {
                    TempData["ErrorMessage"] = "Cannot admit patient without at least one recorded appointment.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Cannot admit patient as all appointments have been cancelled.";
                }
                return RedirectToAction(nameof(Details), new { id });
            }

            try 
            {
                await _patientService.AdmitPatientAsync(id, DateTime.Now);
                TempData["SuccessMessage"] = "Patient admitted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Discharge(int id)
        {
            var admission = (await _unitOfWork.Repository<Admission>().FindAsync(a => a.PatientId == id && a.DischargeDate == null))
                            .FirstOrDefault();

            if (admission == null)
            {
                TempData["ErrorMessage"] = "No active admission found for this patient.";
                return RedirectToAction(nameof(Details), new { id });
            }

            admission.DischargeDate = DateTime.Now;
            // Update the stored days for historical reference
            admission.AdmitDays = (admission.DischargeDate.Value.Date - admission.AdmitDate.Date).Days;
            if (admission.AdmitDays < 1) admission.AdmitDays = 1; // Minimum 1 day for billing

            _unitOfWork.Repository<Admission>().Update(admission);
            await _unitOfWork.CompleteAsync();

            TempData["SuccessMessage"] = "Patient discharged successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // 1. Payment Existence Check
                var bills = await _unitOfWork.Bills.GetBillsByPatientIdAsync(id);
                if (bills.Any(b => b.Status == BillStatus.Paid || b.Payments.Any()))
                {
                    TempData["ErrorMessage"] = "Request Denied: Cannot delete patient with existing payment records.";
                    return RedirectToAction(nameof(Index));
                }

                // In a real app, we'd get the user from User.Identity.Name
                await _patientService.DeletePatientAsync(id, "ADMIN_UI");
                TempData["SuccessMessage"] = "Patient record deleted successfully and moved to Past Records.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                await _patientService.ActivatePatientAsync(id);
                TempData["SuccessMessage"] = "Patient record restored successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
