using BillingSystem.DTOs;
using BillingSystem.Models;
using BillingSystem.Repositories;
using BillingSystem.Services;
using BillingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BillingSystem.Controllers
{
    [Authorize(Roles = "Billing")]
    public class BillingController : Controller
    {
        private readonly IBillingService _billingService;
        private readonly IPatientService _patientService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInsuranceService _insuranceService;

        public BillingController(IBillingService billingService, IPatientService patientService, IUnitOfWork unitOfWork, IInsuranceService insuranceService)
        {
            _billingService = billingService;
            _patientService = patientService;
            _unitOfWork = unitOfWork;
            _insuranceService = insuranceService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchPatient(string query)
        {
            try 
            {
                if (string.IsNullOrEmpty(query)) return Json(new { success = false, message = "Query is empty" });

                // Try ID first
                if (int.TryParse(query, out int id))
                {
                    var patient = await _patientService.GetPatientDetailsAsync(id);
                    if (patient != null) return Json(new { success = true, id = patient.PatientId });
                }

                // Try mobile
                var pMobile = await _patientService.GetPatientByMobileAsync(query);
                if (pMobile != null) return Json(new { success = true, id = pMobile.PatientId });

                return Json(new { success = false, message = "Patient not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server Error: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateBill(int? patientId)
        {
            if (patientId == null) return RedirectToAction("Index", "Home");

            var patient = await _unitOfWork.Patients.GetPatientWithDetailsAsync(patientId.Value);
            if (patient == null) return NotFound();

            // Guard: Check for existing UNPAID bill
            var existingUnpaidBill = patient.Bills.FirstOrDefault(b => b.Status == BillStatus.Unpaid);
            if (existingUnpaidBill != null)
            {
                TempData["WarningMessage"] = "This patient already has an unpaid bill. Please settle it before generating a new one.";
                return RedirectToAction("Payment", new { billId = existingUnpaidBill.BillId });
            }

            var services = await _billingService.GetAllServicesAsync();

            // Filter out lab/diagnostic tests from manual override (keep things like Nursing, Room, Ambulance, etc.)
            var nonLabServices = services
                .Where(s => s.Department != "Pathology" && 
                            s.Department != "Radiology" && 
                            s.Department != "Diagnostics" && 
                            s.Department != "Cardiology");

            var activeInsurance = patient.Insurances.FirstOrDefault(i => i.IsActive);

            var model = new BillGenerationViewModel
            {
                PatientId = patient.PatientId,
                PatientName = patient.FullName,
                Age = patient.Age,
                IsSenior = patient.IsSenior,
                HasInsurance = activeInsurance != null,
                ProviderName = activeInsurance?.ProviderName,
                PolicyNumber = activeInsurance?.PolicyNumber,
                CoverageType = activeInsurance?.CoverageType ?? InsuranceCoverageType.FullBill,
                CoveragePercent = (decimal)(activeInsurance?.CoveragePercent ?? 0),
                AvailableServices = nonLabServices.Select(s => new SelectListItem { Value = s.ServiceId.ToString(), Text = $"{s.ServiceName} ({s.Cost:C})" }).ToList(),
                AvailableProviders = (await _insuranceService.GetLinkedProvidersAsync()).Select(p => new SelectListItem { Value = p.ProviderName, Text = p.ProviderName }).ToList()
            };

            // Automated Calculation for initial load
            var request = new BillCalculationRequestDto 
            { 
                PatientId = patientId.Value, 
                ApplyInsurance = model.HasInsurance,
                PolicyNumber = model.PolicyNumber,
                ProviderName = model.ProviderName
            };
            var result = await _billingService.CalculateBillAsync(request);
            
            model.BillPreview = new BillGenerationPreviewViewModel
            {
                ConsultationFee = result.BaseConsultationFee,
                AdmitAmount = result.AdmitFeeTotal,
                OptionalServicesAmount = result.OptionalServicesTotal,
                GrossTotal = result.GrossTotal,
                Discounts = result.DiscountAmount + result.SeniorDiscountAmount,
                InsuranceDeduction = result.InsuranceDeduction,
                TaxAmount = result.TaxAmount,
                FinalAmount = result.FinalNetPayable,
                Items = result.Items
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePreview(int patientId, bool applyInsurance, List<int> selectedServiceIds, InsuranceCoverageType? insuranceType, double? insurancePercent, string? policyNumber, string? providerName)
        {
             var request = new BillCalculationRequestDto
             {
                 PatientId = patientId,
                 ApplyInsurance = applyInsurance,
                 SelectedServiceIds = selectedServiceIds,
                 InsuranceType = insuranceType,
                 InsurancePercent = insurancePercent,
                 PolicyNumber = policyNumber,
                 ProviderName = providerName
             };
             
             try 
             {
                 var result = await _billingService.CalculateBillAsync(request);
                 
                 var preview = new BillGenerationPreviewViewModel
                 {
                     ConsultationFee = result.BaseConsultationFee,
                     AdmitAmount = result.AdmitFeeTotal,
                     OptionalServicesAmount = result.OptionalServicesTotal,
                     GrossTotal = result.GrossTotal,
                     Discounts = result.DiscountAmount + result.SeniorDiscountAmount,
                     InsuranceDeduction = result.InsuranceDeduction,
                     TaxAmount = result.TaxAmount,
                     FinalAmount = result.FinalNetPayable,
                     Items = result.Items
                 };
                 
                 return PartialView("_BillPreview", preview);
             }
             catch(Exception ex)
             {
                 return BadRequest(ex.Message);
             }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBill(BillGenerationViewModel model)
        {
            var request = new BillCalculationRequestDto
            {
                PatientId = model.PatientId,
                ApplyInsurance = model.HasInsurance,
                SelectedServiceIds = model.SelectedServiceIds,
                InsuranceType = model.CoverageType,
                InsurancePercent = (double)model.CoveragePercent,
                PolicyNumber = model.PolicyNumber,
                ProviderName = model.ProviderName
            };

            try 
            {
                int billId = await _billingService.GenerateBillAsync(request);
                return RedirectToAction("Payment", new { billId = billId });
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Error generating bill: " + ex.Message);
                
                // Reload the model for the view
                var patient = await _patientService.GetPatientDetailsAsync(model.PatientId);
                if (patient != null)
                {
                    model.PatientName = patient.FullName;
                    model.Age = patient.Age;
                    model.IsSenior = patient.IsSenior;
                    model.HasInsurance = patient.Insurances.Any(i => i.IsActive);
                    
                    var calcRequest = new BillCalculationRequestDto { PatientId = model.PatientId, ApplyInsurance = model.HasInsurance };
                    var result = await _billingService.CalculateBillAsync(calcRequest);
                    
                    var allServices = await _billingService.GetAllServicesAsync();
                    var nonLabServices = allServices
                        .Where(s => s.Department != "Pathology" && 
                                    s.Department != "Radiology" && 
                                    s.Department != "Diagnostics" && 
                                    s.Department != "Cardiology");

                    model.AvailableServices = nonLabServices.Select(s => new SelectListItem { Value = s.ServiceId.ToString(), Text = $"{s.ServiceName} ({s.Cost:C})" }).ToList();
                    model.AvailableProviders = (await _insuranceService.GetLinkedProvidersAsync()).Select(p => new SelectListItem { Value = p.ProviderName, Text = p.ProviderName }).ToList();

                    model.BillPreview = new BillGenerationPreviewViewModel
                    {
                        ConsultationFee = result.BaseConsultationFee,
                        AdmitAmount = result.AdmitFeeTotal,
                        OptionalServicesAmount = result.OptionalServicesTotal,
                        GrossTotal = result.GrossTotal,
                        Discounts = result.DiscountAmount + result.SeniorDiscountAmount,
                        InsuranceDeduction = result.InsuranceDeduction,
                        TaxAmount = result.TaxAmount,
                        FinalAmount = result.FinalNetPayable,
                        Items = result.Items
                    };
                }
                
                return View("GenerateBill", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int billId)
        {
            var bill = await _billingService.GetBillByIdAsync(billId);
            if (bill == null) return NotFound();

            var totalPaid = bill.Payments?.Sum(p => p.PaidAmount) ?? 0;
            var pending = bill.FinalAmount - totalPaid;

            if (pending <= 0 && bill.Status == Models.BillStatus.Paid)
            {
                 // Already paid
                 return RedirectToAction("Index", "Home");
            }

            var model = new PaymentViewModel
            {
                BillId = billId,
                PatientName = bill.Patient?.FullName ?? "Unknown",
                TotalAmount = bill.FinalAmount,
                PendingAmount = pending,
                AmountToPay = pending, // Default to full pending
                PaymentMode = Models.PaymentMode.Cash
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Payment(PaymentViewModel model, [FromServices] IPaymentService paymentService)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    var payment = new Models.Payment
                    {
                        BillId = model.BillId,
                        PaymentMode = model.PaymentMode,
                        PaidAmount = model.AmountToPay,
                        TransactionRef = model.TransactionReference
                    };

                    await paymentService.ProcessPaymentAsync(payment);

                    // Redirect to Home
                    return RedirectToAction("Index", "Home"); 
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> PastBills(int patientId)
        {
            var patient = await _unitOfWork.Patients.GetPatientWithDetailsAsync(patientId);
            if (patient == null) return NotFound();

            var bills = await _unitOfWork.Bills.GetBillsByPatientIdAsync(patientId);
            
            ViewBag.PatientName = patient.FullName;
            ViewBag.PatientId = patientId;

            return View(bills.OrderByDescending(b => b.BillDate));
        }
    }
}
