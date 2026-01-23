using BillingSystem.Models;
using BillingSystem.Repositories;

namespace BillingSystem.Services
{
    public interface IInsuranceService
    {
        bool ValidateInsurance(Insurance insurance);
    }

    public class InsuranceService : IInsuranceService
    {
        public bool ValidateInsurance(Insurance insurance)
        {
            if (insurance.CoveragePercent < 0 || insurance.CoveragePercent > 100)
                return false;
            
            if (string.IsNullOrEmpty(insurance.ProviderName))
                return false;

            return true;
        }
    }
}
