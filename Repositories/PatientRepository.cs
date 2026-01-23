using BillingSystem.Data;
using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<bool> IsMobileNumberExistsAsync(string mobileNumber);
        Task<Patient?> GetPatientWithDetailsAsync(int id);
    }

    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(HospitalDbContext context) : base(context)
        {
        }

        public async Task<bool> IsMobileNumberExistsAsync(string mobileNumber)
        {
            return await _context.Patients.AnyAsync(p => p.MobileNumber == mobileNumber);
        }

        public async Task<Patient?> GetPatientWithDetailsAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.Insurances)
                .Include(p => p.Admissions)
                .Include(p => p.PatientServices).ThenInclude(ps => ps.ServiceMaster)
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }
    }
}
