using BillingSystem.DTOs;
using BillingSystem.Models;
using BillingSystem.Repositories;

namespace BillingSystem.Services
{
    public interface IBillingService
    {
        Task<BillCalculationResultDto> CalculateBillAsync(BillCalculationRequestDto request);
        Task<int> GenerateBillAsync(BillCalculationRequestDto request);
        Task<Bill?> GetBillByIdAsync(int billId); // Return Entity or DTO? Entity for now to populate VM easily
        Task<IEnumerable<ServiceMaster>> GetAllServicesAsync();
    }

    public class BillingService : IBillingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const decimal BaseConsultationFee = 500m;

        public BillingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BillCalculationResultDto> CalculateBillAsync(BillCalculationRequestDto request)
        {
            var result = new BillCalculationResultDto
            {
                BaseConsultationFee = BaseConsultationFee
            };
            
            // Get Patient
            var patient = await _unitOfWork.Patients.GetPatientWithDetailsAsync(request.PatientId);
            if (patient == null) throw new Exception("Patient not found");
            result.PatientName = patient.FullName;

            // 1. Consultation Fee
            result.Items.Add(new BillItemDto 
            { 
                Description = "Consultation Fee", 
                Amount = BaseConsultationFee, 
                Type = BillItemType.Consultation 
            });

            // 2. Optional Services
            decimal servicesTotal = 0;
            if (request.SelectedServiceIds != null && request.SelectedServiceIds.Any())
            {
                // In a real app, optimize this query
                var services = await _unitOfWork.Services.FindAsync(s => request.SelectedServiceIds.Contains(s.ServiceId));
                
                foreach (var service in services)
                {
                    // Assuming quantity 1 for now as per DTO simple list, usually map is better
                    servicesTotal += service.Cost; 
                    result.Items.Add(new BillItemDto
                    {
                        Description = service.ServiceName,
                        Amount = service.Cost,
                        Type = BillItemType.Service
                    });
                }
            }
            result.OptionalServicesTotal = servicesTotal;

            // 3. Admit Charges
            decimal admitTotal = 0;
            if (request.AdmitDays > 0)
            {
                // Check if patient has admission record or just calc? 
                // Requirement DTO has AdmitDays. We use that for calculation.
                // Default 2000 as per prompt
                decimal feePerDay = 2000m; 
                admitTotal = request.AdmitDays * feePerDay;
                result.AdmitFeeTotal = admitTotal;
                
                result.Items.Add(new BillItemDto
                {
                    Description = $"Room Charges ({request.AdmitDays} days)",
                    Amount = admitTotal,
                    Type = BillItemType.Admission
                });
            }

            // 4. Gross Total
            result.GrossTotal = BaseConsultationFee + servicesTotal + admitTotal;

            // 5. Discount Rules
            decimal currentTotal = result.GrossTotal;

            // 5% if > 2000
            if (result.GrossTotal > 2000)
            {
                result.DiscountAmount = result.GrossTotal * 0.05m;
                currentTotal -= result.DiscountAmount;
                result.Items.Add(new BillItemDto { Description = "High Value Discount (5%)", Amount = -result.DiscountAmount, Type = BillItemType.Discount });
            }

            // Senior Citizen (10% on remaining)
            if (patient.IsSenior)
            {
                result.SeniorDiscountAmount = currentTotal * 0.10m;
                currentTotal -= result.SeniorDiscountAmount;
                 result.Items.Add(new BillItemDto { Description = "Senior Citizen Discount (10%)", Amount = -result.SeniorDiscountAmount, Type = BillItemType.Discount });
            }

            // 6. Insurance
            // "Insurance applied AFTER discounts"
            if (request.ApplyInsurance)
            {
                // Prioritize UI-provided insurance details
                string? provider = null;
                double? percent = request.InsurancePercent;
                InsuranceCoverageType? type = request.InsuranceType;

                // Fallback to database if request is incomplete
                var dbInsurance = patient.Insurances.FirstOrDefault(i => i.IsActive);
                if (dbInsurance != null)
                {
                    provider ??= dbInsurance.ProviderName;
                    percent ??= dbInsurance.CoveragePercent;
                    type ??= dbInsurance.CoverageType;
                }

                if (type.HasValue && percent.HasValue)
                {
                    result.InsuranceProvider = provider ?? "Manual Entry";
                    result.CoveragePercent = percent.Value;
                    
                    decimal insuranceBase = 0;
                    switch (type.Value)
                    {
                        case InsuranceCoverageType.FullBill:
                            insuranceBase = currentTotal;
                            break;
                         case InsuranceCoverageType.OptionalServicesOnly:
                            insuranceBase = result.OptionalServicesTotal; 
                            break;
                         case InsuranceCoverageType.AdmitFeeOnly:
                            insuranceBase = result.AdmitFeeTotal;
                            break;
                         case InsuranceCoverageType.ConsultationFeeOnly:
                            insuranceBase = result.BaseConsultationFee;
                            break;
                    }

                    // Safety: Percent should be between 0 and 100
                    var safePercent = Math.Max(0, Math.Min(100, percent.Value));
                    result.InsuranceDeduction = insuranceBase * (decimal)(safePercent / 100.0);
                    
                    // Cap insurance deduction if it exceeds the remaining bill
                    if(result.InsuranceDeduction > currentTotal) result.InsuranceDeduction = currentTotal;

                    currentTotal -= result.InsuranceDeduction;
                    
                    result.Items.Add(new BillItemDto { Description = $"Insurance Coverage ({type})", Amount = -result.InsuranceDeduction, Type = BillItemType.Insurance });
                }
            }
            
            if (currentTotal < 0) currentTotal = 0;

            // 7. Tax (8% on remaining)
            result.TaxAmount = currentTotal * 0.08m;
            result.Items.Add(new BillItemDto { Description = "Tax (8%)", Amount = result.TaxAmount, Type = BillItemType.Tax });

            // 8. Final
            result.FinalNetPayable = currentTotal + result.TaxAmount;

            return result;
        }

        public async Task<int> GenerateBillAsync(BillCalculationRequestDto request)
        {
            var calc = await CalculateBillAsync(request);

            try 
            {
                await _unitOfWork.BeginTransactionAsync();

                var bill = new Bill
                {
                    PatientId = request.PatientId,
                    BillDate = DateTime.Now,
                    ConsultationFee = calc.BaseConsultationFee,
                    OptionalServicesAmount = calc.OptionalServicesTotal,
                    AdmitAmount = calc.AdmitFeeTotal,
                    TotalAmount = calc.GrossTotal,
                    DiscountAmount = calc.DiscountAmount,
                    SeniorDiscount = calc.SeniorDiscountAmount,
                    InsuranceDeduction = calc.InsuranceDeduction,
                    TaxAmount = calc.TaxAmount, // Warning: Calc DTO has tax separated, entity has tax field? Yes.
                    FinalAmount = calc.FinalNetPayable,
                    Status = BillStatus.Unpaid
                };

                await _unitOfWork.Repository<Bill>().AddAsync(bill);
                await _unitOfWork.CompleteAsync(); // Get ID

                foreach(var item in calc.Items)
                {
                    var billItem = new BillItem
                    {
                        BillId = bill.BillId,
                        Description = item.Description,
                        Amount = item.Amount, // Can be negative for discounts
                        ItemType = item.Type
                    };
                    await _unitOfWork.Repository<BillItem>().AddAsync(billItem);
                }

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
                
                return bill.BillId;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<Bill?> GetBillByIdAsync(int billId)
        {
            return await _unitOfWork.Bills.GetBillWithDetailsAsync(billId);
        }

        public async Task<IEnumerable<ServiceMaster>> GetAllServicesAsync()
        {
            return await _unitOfWork.Services.GetAllAsync();
        }
    }
}
