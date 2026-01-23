using BillingSystem.DTOs;
using BillingSystem.Models;
using BillingSystem.Services;
using BillingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BillingSystem.Controllers
{
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
                // 1. Create Patient Entity
                var patient = new Patient
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    MobileNumber = model.MobileNumber,
                    Email = model.Email,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = DateTime.Now
                };

                // 2. Admission (Optional at registration)
                if (model.AdmitDays > 0)
                {
                    patient.Admissions.Add(new Admission
                    {
                        AdmitDays = model.AdmitDays,
                        FeePerDay = 2000, 
                        AdmitDate = DateTime.Now,
                        DischargeDate = DateTime.Now.AddDays(model.AdmitDays)
                    });
                }

                try 
                {
                    // 3. Register via Service
                    var patientId = await _patientService.RegisterPatientAsync(patient);

                    // Redirect to Billing Desk where services/insurance are handled
                    return RedirectToAction("BillingDesk", "CashierBilling", new { patientId = patientId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(model);
        }
    }
}
