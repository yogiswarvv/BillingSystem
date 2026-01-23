using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BillingSystem.Services;
using BillingSystem.ViewModels;
using BillingSystem.DTOs;
using BillingSystem.Models;

namespace BillingSystem.Controllers
{
    // [Authorize(Roles = "Cashier,BillingStaff")] // Commented out for now as Roles might not be set up in Identity yet, but added as per prompt requirement
    public class CashierBillingController : Controller
    {
        private readonly IBillingService _billingService;
        private readonly IPatientService _patientService;
        private readonly IPaymentService _paymentService;

        public CashierBillingController(IBillingService billingService, IPatientService patientService, IPaymentService paymentService)
        {
            _billingService = billingService;
            _patientService = patientService;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<IActionResult> BillingDesk()
        {
            var services = await _billingService.GetAllServicesAsync();
            var model = new CashierBillingViewModel
            {
                AvailableServices = services.Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = $"{s.ServiceName} ({s.Cost:C})"
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PreviewBill(CashierBillingViewModel model)
        {
            // Re-populate services for the view if returned
            var services = await _billingService.GetAllServicesAsync();
            model.AvailableServices = services.Select(s => new SelectListItem
            {
                Value = s.ServiceId.ToString(),
                Text = $"{s.ServiceName} ({s.Cost:C})"
            }).ToList();

            if (!ModelState.IsValid)
            {
                return View("BillingDesk", model);
            }

            var request = new BillCalculationRequestDto
            {
                PatientId = model.PatientId,
                SelectedServiceIds = model.SelectedServiceIds,
                AdmitDays = model.AdmitDays,
                ApplyInsurance = model.HasInsurance,
                InsuranceType = model.InsuranceCoverageType,
                InsurancePercent = model.InsurancePercent,
                SelectedLabOrderIds = model.SelectedLabOrderIds
            };

            try 
            {
                var result = await _billingService.CalculateBillAsync(request);

                model.BillPreview = new BillPreviewViewModel
                {
                    ConsultationFee = result.BaseConsultationFee,
                    ServicesTotal = result.OptionalServicesTotal,
                    AdmitAmount = result.AdmitFeeTotal,
                    DiscountAmount = result.DiscountAmount,
                    SeniorDiscount = result.SeniorDiscountAmount,
                    InsuranceDeduction = result.InsuranceDeduction,
                    TaxAmount = result.TaxAmount,
                    FinalAmount = result.FinalNetPayable
                };
                
                // Refresh patient info from result/service to ensure UI is consistent
                var patient = await _patientService.GetPatientDetailsAsync(model.PatientId);
                if (patient != null)
                {
                    model.PatientName = patient.FullName;
                    model.Age = patient.Age;
                    model.IsSenior = patient.IsSenior;
                }
                
                return View("BillingDesk", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error calculating bill: {ex.Message}");
                return View("BillingDesk", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBill(CashierBillingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-populate services
                var services = await _billingService.GetAllServicesAsync();
                model.AvailableServices = services.Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = $"{s.ServiceName} ({s.Cost:C})"
                }).ToList();
                return View("BillingDesk", model);
            }

            var request = new BillCalculationRequestDto
            {
                PatientId = model.PatientId,
                SelectedServiceIds = model.SelectedServiceIds,
                AdmitDays = model.AdmitDays,
                ApplyInsurance = model.HasInsurance,
                InsuranceType = model.InsuranceCoverageType,
                InsurancePercent = model.InsurancePercent,
                SelectedLabOrderIds = model.SelectedLabOrderIds
            };

            try
            {
                var billId = await _billingService.GenerateBillAsync(request);
                return RedirectToAction("Payment", new { billId = billId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error generating bill: {ex.Message}");
                 // Re-populate services
                var services = await _billingService.GetAllServicesAsync();
                model.AvailableServices = services.Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = $"{s.ServiceName} ({s.Cost:C})"
                }).ToList();
                return View("BillingDesk", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int billId)
        {
            var bill = await _billingService.GetBillByIdAsync(billId);
            if (bill == null) return NotFound();

            var model = new PaymentViewModel
            {
                BillId = bill.BillId,
                TotalBillAmount = bill.FinalAmount,
                PendingAmount = bill.Status == BillStatus.Paid ? 0 : bill.FinalAmount, // Simple logic, assumes no partial payments yet for this scope
                PaidAmount = bill.Status == BillStatus.Paid ? 0 : bill.FinalAmount // Default to full amount
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CollectPayment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Payment", model);
            }

            try
            {
                var payment = new Payment
                {
                    BillId = model.BillId,
                    PaymentMode = model.PaymentMode,
                    PaidAmount = model.PaidAmount,
                    TransactionRef = model.TransactionReference
                };

                await _paymentService.ProcessPaymentAsync(payment);

                return RedirectToAction("PaymentSuccess", new { billId = model.BillId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Payment processing failed: {ex.Message}");
                return View("Payment", model);
            }
        }
        
        [HttpGet]
        public IActionResult PaymentSuccess(int billId)
        {
            ViewBag.BillId = billId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Invoice(int billId)
        {
            var bill = await _billingService.GetBillByIdAsync(billId);
            if (bill == null) return NotFound();

            return View(bill);
        }
    }
}
