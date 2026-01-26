using BillingSystem.Models;
using BillingSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingSystem.Services
{
    public interface IInsuranceService
    {
        Task<FederatedMemberDetails?> ValidateRegistryMemberAsync(string providerName, string policyNumber, string fullName, DateTime dob);
        Task<decimal> CalculateCoverageAsync(string providerName, string policyNumber, decimal billAmount);
        Task<int> SubmitClaimAsync(int billId, string policyNumber, decimal requestedAmount);
        Task<IEnumerable<InsuranceProvider>> GetLinkedProvidersAsync();
        Task<FederatedMemberDetails?> GetMemberDetailsAsync(string policyNumber);
    }

    public class InsuranceService : IInsuranceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsuranceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FederatedMemberDetails?> ValidateRegistryMemberAsync(string providerName, string policyNumber, string fullName, DateTime dob)
        {
            var member = await GetMemberFromCorrectRegistryAsync(providerName, policyNumber);
            if (member == null) return null;

            // Check Demographics (Case-Insensitive)
            if (member.FullName.Trim().Equals(fullName.Trim(), StringComparison.OrdinalIgnoreCase) &&
                member.DateOfBirth.Date == dob.Date &&
                member.Status == "Active")
            {
                return member;
            }

            return null;
        }

        public async Task<decimal> CalculateCoverageAsync(string providerName, string policyNumber, decimal billAmount)
        {
            var member = await GetMemberFromCorrectRegistryAsync(providerName, policyNumber);
            if (member == null) return 0;

            var plan = await _unitOfWork.Repository<InsurancePlan>().GetByIdAsync(member.PlanID);
            if (plan == null) return 0;

            // 1. Calculate Coverage based on Plan %
            decimal planShare = billAmount * (plan.CoveragePercentage / 100m);
            
            // 2. Subtract Co-Pay if applicable
            decimal netShare = planShare - plan.CoPayAmount;
            if (netShare < 0) netShare = 0;

            // 3. Cap by Max Benefit per Claim
            if (netShare > plan.MaxBenefitPerClaim) netShare = plan.MaxBenefitPerClaim;

            // 4. Cap by Remaining Balance
            if (netShare > member.RemainingBalance) netShare = member.RemainingBalance;

            return netShare;
        }

        private async Task<FederatedMemberDetails?> GetMemberFromCorrectRegistryAsync(string providerName, string policyNumber)
        {
            if (string.IsNullOrEmpty(providerName)) return null;

            if (providerName.Contains("Apollo", StringComparison.OrdinalIgnoreCase))
            {
                var m = (await _unitOfWork.Repository<ApolloMunichRegistry>().FindAsync(x => x.PolicyNumber == policyNumber)).FirstOrDefault();
                return m == null ? null : MapToFederated(m, "Apollo Munich");
            }
            else if (providerName.Contains("HDFC", StringComparison.OrdinalIgnoreCase))
            {
                var m = (await _unitOfWork.Repository<HDFCErgoRegistry>().FindAsync(x => x.PolicyNumber == policyNumber)).FirstOrDefault();
                return m == null ? null : MapToFederated(m, "HDFC Ergo");
            }
            else if (providerName.Contains("Star", StringComparison.OrdinalIgnoreCase))
            {
                var m = (await _unitOfWork.Repository<StarHealthRegistry>().FindAsync(x => x.PolicyNumber == policyNumber)).FirstOrDefault();
                return m == null ? null : MapToFederated(m, "Star Health");
            }

            return null;
        }

        private FederatedMemberDetails MapToFederated(BaseInsuranceRegistry m, string provider)
        {
            return new FederatedMemberDetails
            {
                MemberID = m.MemberID,
                PolicyNumber = m.PolicyNumber,
                FullName = m.FullName,
                DateOfBirth = m.DateOfBirth,
                PlanID = m.PlanID,
                RemainingBalance = m.RemainingBalance,
                Status = m.Status,
                ProviderName = provider
            };
        }

        public async Task<int> SubmitClaimAsync(int billId, string policyNumber, decimal requestedAmount)
        {
            // Update Balance in correct table
            if (policyNumber.StartsWith("AM"))
            {
                var m = (await _unitOfWork.Repository<ApolloMunichRegistry>().FindAsync(x => x.PolicyNumber == policyNumber)).FirstOrDefault();
                if (m != null) { m.RemainingBalance -= requestedAmount; _unitOfWork.Repository<ApolloMunichRegistry>().Update(m); }
            }
            else if (policyNumber.StartsWith("HE"))
            {
                var m = (await _unitOfWork.Repository<HDFCErgoRegistry>().FindAsync(x => x.PolicyNumber == policyNumber)).FirstOrDefault();
                if (m != null) { m.RemainingBalance -= requestedAmount; _unitOfWork.Repository<HDFCErgoRegistry>().Update(m); }
            }
            else if (policyNumber.StartsWith("SH"))
            {
                var m = (await _unitOfWork.Repository<StarHealthRegistry>().FindAsync(x => x.PolicyNumber == policyNumber)).FirstOrDefault();
                if (m != null) { m.RemainingBalance -= requestedAmount; _unitOfWork.Repository<StarHealthRegistry>().Update(m); }
            }

            var claim = new InsuranceClaim
            {
                BillID = billId,
                PolicyNumber = policyNumber,
                ClaimAmountRequested = requestedAmount,
                ApprovedAmount = requestedAmount,
                ClaimStatus = "Approved",
                ProcessedDate = DateTime.Now,
                ApprovalCode = "APP-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
            };

            await _unitOfWork.Repository<InsuranceClaim>().AddAsync(claim);
            await _unitOfWork.CompleteAsync();
            return claim.ClaimID;
        }

        public async Task<IEnumerable<InsuranceProvider>> GetLinkedProvidersAsync()
        {
            return (await _unitOfWork.Repository<InsuranceProvider>().GetAllAsync())
                .Where(p => p.IsLinked)
                .OrderBy(p => p.ProviderName);
        }

        public async Task<FederatedMemberDetails?> GetMemberDetailsAsync(string policyNumber)
        {
            if (string.IsNullOrEmpty(policyNumber)) return null;

            // Try all registries for search
            FederatedMemberDetails? member = null;
            if (policyNumber.StartsWith("AM")) member = await GetMemberFromCorrectRegistryAsync("Apollo Munich", policyNumber);
            else if (policyNumber.StartsWith("HE")) member = await GetMemberFromCorrectRegistryAsync("HDFC Ergo", policyNumber);
            else if (policyNumber.StartsWith("SH")) member = await GetMemberFromCorrectRegistryAsync("Star Health", policyNumber);
            
            return member;
        }
    }
}
