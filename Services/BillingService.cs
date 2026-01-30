using BillingSystem.DTOs;
using BillingSystem.Models;
using BillingSystem.Repositories;

namespace BillingSystem.Services
{
    public interface IBillingService
    {
        Task<BillCalculationResultDto> CalculateBillAsync(BillCalculationRequestDto request);
        Task<int> GenerateBillAsync(BillCalculationRequestDto request);
        Task<Bill?> GetBillByIdAsync(int billId);
        Task<IEnumerable<Bill>> GetBillsByPatientIdAsync(int patientId);
        Task<IEnumerable<ServiceMaster>> GetAllServicesAsync();
        Task CompleteBillItemsAsync(int billId);
        Task<Patient?> GetPatientForBillingAsync(string searchTerm);
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

            // 1. Consultation Fee (Appointments)
            decimal consultationFee = 0;
            var unpaidAppointments = patient.Appointments.Where(a => !a.IsPaid && a.Status != "Cancelled").ToList();
            foreach (var appt in unpaidAppointments)
            {
                consultationFee += BaseConsultationFee;
                result.Items.Add(new BillItemDto 
                { 
                    Description = $"Consultation Fee (Appt: {appt.AppointmentDate.ToShortDateString()})", 
                    Amount = BaseConsultationFee, 
                    Type = BillItemType.Consultation 
                });
            }
            result.BaseConsultationFee = consultationFee;

            // 2. Lab Orders
            decimal labTotal = 0;
            var unpaidLabs = patient.Appointments
                .Where(a => a.Status != "Cancelled")
                .SelectMany(a => a.LabOrders)
                .Where(l => !l.IsPaid) // include both Pending and Completed if unpaid
                .ToList();

            var services = await _unitOfWork.Services.GetAllAsync();

            foreach (var lab in unpaidLabs)
            {
                var service = services.FirstOrDefault(s => s.ServiceName.Trim().Equals(lab.TestName.Trim(), StringComparison.OrdinalIgnoreCase));
                decimal labCost = service?.Cost ?? 0;
                
                labTotal += labCost;
                result.Items.Add(new BillItemDto
                {
                    Description = $"Lab: {lab.TestName}",
                    Amount = labCost,
                    Type = BillItemType.Service
                });
            }
            result.OptionalServicesTotal = labTotal;

            // 3. Pharmacy Charges (Prescriptions)
            decimal pharmacyTotal = 0;
            var unpaidPrescriptions = patient.Appointments
                .Where(a => a.Status != "Cancelled")
                .SelectMany(a => a.Prescriptions)
                .Where(p => !p.IsPaid)
                .ToList();

            foreach (var pre in unpaidPrescriptions)
            {
                var medicine = await _unitOfWork.Repository<Medicine>().GetByIdAsync(pre.MedicineId);
                if (medicine != null)
                {
                    var amount = pre.ActualQuantity * medicine.PricePerUnit;
                    pharmacyTotal += amount;
                    result.Items.Add(new BillItemDto
                    {
                        Description = $"Med: {medicine.Name}",
                        Amount = amount,
                        Type = BillItemType.Service,
                        ReferenceId = pre.PrescriptionId,
                        SuggestedQuantity = pre.SuggestedQuantity,
                        ActualQuantity = pre.ActualQuantity,
                        UnitPrice = medicine.PricePerUnit
                    });
                }
            }
            result.PharmacyTotal = pharmacyTotal;

            // 4. Admit Charges
            decimal admitTotal = 0;
            var unpaidAdmissions = patient.Admissions.Where(a => !a.IsPaid).ToList();
            foreach (var admit in unpaidAdmissions)
            {
                var days = (admit.DischargeDate ?? DateTime.Now).Date.Subtract(admit.AdmitDate.Date).Days;
                if (days < 1) days = 1; // Min 1 day
                
                var amount = days * admit.FeePerDay;
                admitTotal += amount;
                result.Items.Add(new BillItemDto
                {
                    Description = $"Room Charges ({days} days @ {admit.FeePerDay:C})",
                    Amount = amount,
                    Type = BillItemType.Admission
                });
            }
            result.AdmitFeeTotal = admitTotal;

            // 5. Gross Total
            result.GrossTotal = consultationFee + labTotal + pharmacyTotal + admitTotal;

            decimal currentTotal = result.GrossTotal;
            decimal insurableTotal = 0;

            if (request.ApplyInsuranceToConsultation) insurableTotal += consultationFee;
            if (request.ApplyInsuranceToLabs) insurableTotal += labTotal;
            if (request.ApplyInsuranceToPharmacy) insurableTotal += pharmacyTotal;
            if (request.ApplyInsuranceToAdmission) insurableTotal += admitTotal;

            // 5. Discount Rules
            // ... (rest of discounts)
            if (result.GrossTotal > 2000)
            {
                result.DiscountAmount = result.GrossTotal * 0.05m;
                currentTotal -= result.DiscountAmount;
                // Note: Discounts are usually applied proportionally if insurance is partial, 
                // but here we calculate insurance on the category totals.
                result.Items.Add(new BillItemDto { Description = "High Value Discount (5%)", Amount = -result.DiscountAmount, Type = BillItemType.Discount });
            }

            // Senior Citizen (10% on remaining)
            if (patient.IsSenior)
            {
                result.SeniorDiscountAmount = currentTotal * 0.10m;
                currentTotal -= result.SeniorDiscountAmount;
                result.Items.Add(new BillItemDto { Description = "Senior Citizen Discount (10%)", Amount = -result.SeniorDiscountAmount, Type = BillItemType.Discount });
            }

            // 6. Production Insurance Integration
            if (request.ApplyInsurance)
            {
                var insuranceService = (IInsuranceService)request.GetType().Assembly.CreateInstance("BillingSystem.Services.InsuranceService", false, System.Reflection.BindingFlags.Default, null, new object[] { _unitOfWork }, null, null)!;
                
                string? policyNo = request.PolicyNumber;
                string? providerName = request.ProviderName;

                if (string.IsNullOrEmpty(policyNo) || string.IsNullOrEmpty(providerName))
                {
                    var dbInsurance = patient.Insurances.FirstOrDefault(i => i.IsActive);
                    if (dbInsurance != null)
                    {
                        policyNo = dbInsurance.PolicyNumber;
                        providerName = dbInsurance.ProviderName;
                    }
                }

                if (!string.IsNullOrEmpty(policyNo) && !string.IsNullOrEmpty(providerName))
                {
                    var validInsurance = await _unitOfWork.Bills.CheckPatientInsuranceSPAsync(patient.PatientId, providerName.Trim(), policyNo.Trim());
                    var member = await insuranceService.ValidateRegistryMemberAsync(providerName, policyNo, patient.FullName, patient.DateOfBirth);

                    if (validInsurance != null || member != null)
                    {
                        decimal effectivePercent = 0;
                        
                        if (request.InsurancePercent.HasValue && request.InsurancePercent.Value > 0)
                        {
                            effectivePercent = (decimal)request.InsurancePercent.Value;
                        }
                        else if (validInsurance != null && validInsurance.CoveragePercent > 0)
                        {
                            effectivePercent = (decimal)validInsurance.CoveragePercent;
                        }

                        if (member != null)
                        {
                            result.InsuranceDeduction = await insuranceService.CalculateCoverageAsync(providerName, policyNo, insurableTotal);
                            result.InsuranceProvider = member.ProviderName; 
                        }

                        bool useMethodB = result.InsuranceDeduction == 0;
                        if (request.InsurancePercent.HasValue && request.InsurancePercent.Value > 0)
                        {
                            useMethodB = true;
                        }
                        else if (result.InsuranceDeduction == 0 && effectivePercent > 0)
                        {
                            useMethodB = true;
                        }

                        if (useMethodB && effectivePercent > 0)
                        {
                             result.InsuranceDeduction = insurableTotal * (effectivePercent / 100m);
                             result.CoveragePercent = (double)effectivePercent; 
                        }

                        if(result.InsuranceDeduction > currentTotal) result.InsuranceDeduction = currentTotal;

                        currentTotal -= result.InsuranceDeduction;
                        
                        var displayProvider = member?.ProviderName ?? validInsurance?.ProviderName ?? providerName;
                        result.InsuranceProvider = displayProvider;
                        
                        result.Items.Add(new BillItemDto 
                        { 
                            Description = $"Insurance Coverage (Policy: {policyNo} via {displayProvider})", 
                            Amount = -result.InsuranceDeduction, 
                            Type = BillItemType.Insurance 
                        });
                        
                        if (result.CoveragePercent == 0 && effectivePercent > 0) result.CoveragePercent = (double)effectivePercent;
                    }
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

                // NOTE: DO NOT MARK ITEMS AS PAID HERE. 
                // Status is only finalized in CompleteBillItemsAsync upon physical payment.
                
                // 3. Finalize Insurance Claims
                if (calc.InsuranceDeduction > 0)
                {
                    var insuranceService = (IInsuranceService)request.GetType().Assembly.CreateInstance("BillingSystem.Services.InsuranceService", false, System.Reflection.BindingFlags.Default, null, new object[] { _unitOfWork }, null, null)!;
                    
                    string? policyNo = request.PolicyNumber;
                    if (string.IsNullOrEmpty(policyNo))
                    {
                         var insurances = await _unitOfWork.Repository<Insurance>().FindAsync(i => i.PatientId == request.PatientId && i.IsActive);
                         policyNo = insurances.FirstOrDefault()?.PolicyNumber;
                    }

                    if (!string.IsNullOrEmpty(policyNo))
                    {
                        await insuranceService.SubmitClaimAsync(bill.BillId, policyNo, calc.InsuranceDeduction);
                    }
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

        public async Task<IEnumerable<Bill>> GetBillsByPatientIdAsync(int patientId)
        {
            return await _unitOfWork.Bills.GetBillsByPatientIdAsync(patientId);
        }

        public async Task CompleteBillItemsAsync(int billId)
        {
            var bill = await _unitOfWork.Bills.GetBillWithDetailsAsync(billId);
            if (bill == null) return;

            var patient = await _unitOfWork.Patients.GetPatientWithDetailsAsync(bill.PatientId);
            if (patient == null) return;

            // Mark Appointments
            foreach (var appt in patient.Appointments.Where(a => !a.IsPaid && a.Status != "Cancelled"))
            {
                appt.IsPaid = true;
                _unitOfWork.Repository<Appointment>().Update(appt);
            }

            // Mark Lab Orders
            foreach (var lab in patient.Appointments.SelectMany(a => a.LabOrders).Where(l => !l.IsPaid))
            {
                lab.IsPaid = true;
                if (lab.Status == "Pending") lab.Status = "Ready for Processing";
                _unitOfWork.Repository<LabOrder>().Update(lab);
            }

            // Mark Prescriptions
            foreach (var pre in patient.Appointments.SelectMany(a => a.Prescriptions).Where(p => !p.IsPaid))
            {
                pre.IsPaid = true;
                pre.Status = "Purchased";
                _unitOfWork.Repository<Prescription>().Update(pre);
            }

            // Mark Admissions
            foreach (var admit in patient.Admissions.Where(a => !a.IsPaid))
            {
                admit.IsPaid = true;
                _unitOfWork.Repository<Admission>().Update(admit);
            }

            await _unitOfWork.CompleteAsync();
        }


        public async Task<Patient?> GetPatientForBillingAsync(string searchTerm)
        {
            return await _unitOfWork.Bills.GetPatientForBillingSPAsync(searchTerm);
        }
    }
}
