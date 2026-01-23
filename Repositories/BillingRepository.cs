using BillingSystem.Data;
using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Repositories
{
    public interface IBillingRepository : IRepository<Bill>
    {
        Task<Bill?> GetBillWithDetailsAsync(int billId);
        Task<IEnumerable<Bill>> GetBillsByPatientIdAsync(int patientId);
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
                .Where(b => b.PatientId == patientId)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync();
        }
    }
}
