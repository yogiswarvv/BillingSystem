using BillingSystem.Data;
using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Repositories
{
    public interface IServiceMasterRepository : IRepository<ServiceMaster>
    {
        Task<IEnumerable<ServiceMaster>> GetActiveServicesAsync();
        Task<bool> IsServiceNameExistsAsync(string serviceName);
    }

    public class ServiceMasterRepository : Repository<ServiceMaster>, IServiceMasterRepository
    {
        public ServiceMasterRepository(HospitalDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ServiceMaster>> GetActiveServicesAsync()
        {
            return await _context.Services.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<bool> IsServiceNameExistsAsync(string serviceName)
        {
            return await _context.Services.AnyAsync(s => s.ServiceName == serviceName);
        }
    }
}
