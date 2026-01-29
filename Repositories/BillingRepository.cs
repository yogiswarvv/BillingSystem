using BillingSystem.Data;
using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace BillingSystem.Repositories
{
    public interface IBillingRepository : IRepository<Bill>
    {
        Task<Bill?> GetBillWithDetailsAsync(int billId);
        Task<IEnumerable<Bill>> GetBillsByPatientIdAsync(int patientId);
        Task<Patient?> GetPatientForBillingSPAsync(string searchTerm);
        Task<Insurance?> CheckPatientInsuranceSPAsync(int patientId, string providerName, string policyNumber);
    }

    public class BillingRepository : Repository<Bill>, IBillingRepository
    {
        public BillingRepository(HospitalDbContext context) : base(context)
        {
        }

        public async Task<Bill?> GetBillWithDetailsAsync(int billId)
        {
            return await _context.Bills
                .Include(b => b.BillItems)
                .Include(b => b.Payments)
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(b => b.BillId == billId);
        }

        public async Task<IEnumerable<Bill>> GetBillsByPatientIdAsync(int patientId)
        {
             return await _context.Bills
                .Include(b => b.BillItems)
                .Include(b => b.Payments)
                .Where(b => b.PatientId == patientId)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientForBillingSPAsync(string searchTerm)
        {
            var pSearch = new SqlParameter("@SearchTerm", searchTerm ?? "");
            
            var result = await _context.Patients
                .FromSqlRaw("EXEC [Healthcare].[usp_GetPatientForBilling] @SearchTerm", pSearch)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task<Insurance?> CheckPatientInsuranceSPAsync(int patientId, string providerName, string policyNumber)
        {
            var pId = new SqlParameter("@PatientId", patientId);
            var pName = new SqlParameter("@ProviderName", providerName ?? "");
            var pPolicy = new SqlParameter("@PolicyNumber", policyNumber ?? "");

            var result = await _context.Set<Insurance>()
                .FromSqlRaw("EXEC [Healthcare].[usp_CheckPatientInsurance] @PatientId, @ProviderName, @PolicyNumber", 
                    pId, pName, pPolicy)
                .ToListAsync();

            return result.FirstOrDefault();
        }
    }
}
