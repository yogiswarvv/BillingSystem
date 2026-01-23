using BillingSystem.Data;
using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Services
{
    public class DoctorService
    {
        private readonly HospitalDbContext _context;

        public DoctorService(HospitalDbContext context)
        {
            _context = context;
        }

        // Fetch ONLY available doctors
        public async Task<List<Doctor>> GetAvailableDoctorsAsync()
        {
            return await _context.Doctors
                .Where(d => d.IsAvailable)
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .ToListAsync();
        }

        // Validate doctor availability before booking
        public async Task<bool> IsDoctorAvailableAsync(int doctorId)
        {
            return await _context.Doctors
                .AnyAsync(d => d.DoctorId == doctorId && d.IsAvailable);
        }
    }
}
