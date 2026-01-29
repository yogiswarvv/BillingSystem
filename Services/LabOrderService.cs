using BillingSystem.Data;
using BillingSystem.Models;
using BillingSystem.Repositories;
using BillingSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BillingSystem.Services
{
    public class LabOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HospitalDbContext _context;

        public LabOrderService(IUnitOfWork unitOfWork, HospitalDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<LabOrder>> SearchLabOrdersAsync(string? searchTerm)
        {
            return await _unitOfWork.Patients.SearchLabOrdersSPAsync(searchTerm);
        }

        public async Task<IEnumerable<LabOrder>> GetAllLabOrdersAsync()
        {
            return await SearchLabOrdersAsync(null);
        }

        public async Task<LabOrder?> GetLabOrderByIdAsync(int id)
        {
            return await _context.LabOrders
                .Include(l => l.Appointment)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(l => l.LabOrderId == id);
        }

        public async Task<IEnumerable<LabOrder>> GetLabOrdersByAppointmentIdAsync(int appointmentId)
        {
            return await _context.LabOrders
                .Where(l => l.AppointmentId == appointmentId)
                .OrderBy(l => l.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabOrder>> GetPendingLabOrdersAsync()
        {
            return await _context.LabOrders
                .Include(l => l.Appointment)
                    .ThenInclude(a => a.Patient)
                .Where(l => l.Status == "Pending")
                .OrderBy(l => l.OrderDate)
                .ToListAsync();
        }

        public async Task CreateLabOrdersAsync(int appointmentId, List<string> testNames)
        {
            if (testNames == null || !testNames.Any())
                throw new ArgumentException("No tests selected.");

            // 1. Fetch Costs
            var services = await _unitOfWork.Repository<ServiceMaster>().GetAllAsync();
            
            // 2. Construct TVP
            var table = new DataTable();
            table.Columns.Add("TestName", typeof(string));
            table.Columns.Add("Cost", typeof(decimal));

            foreach (var testName in testNames)
            {
                var service = services.FirstOrDefault(s => s.ServiceName.Equals(testName, StringComparison.OrdinalIgnoreCase));
                decimal cost = service?.Cost ?? 0;
                table.Rows.Add(testName, cost);
            }

            try
            {
                await _unitOfWork.Patients.OrderLabTestsSPAsync(appointmentId, table);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("500")) throw new InvalidOperationException(ex.Message);
                throw;
            }
        }

        public async Task UpdateLabOrderAsync(LabOrder labOrder)
        {
            var existing = await _unitOfWork.Repository<LabOrder>().GetByIdAsync(labOrder.LabOrderId);
            if (existing == null)
                throw new InvalidOperationException($"Lab order with ID {labOrder.LabOrderId} not found.");

            _unitOfWork.Repository<LabOrder>().Update(labOrder);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateLabOrderStatusAsync(int labOrderId, string status, string? results = null)
        {
            DateTime? completedDate = null;
            if (status == "Completed")
            {
                completedDate = DateTime.Now;
            }

            await _unitOfWork.Patients.UpdateLabResultSPAsync(labOrderId, status, results, completedDate);
        }

        public async Task DeleteLabOrderAsync(int id)
        {
            var existing = await _context.LabOrders.FindAsync(id);
            if (existing != null)
            {
                // Rule: Remove Lab Order Entered by Mistake validation
                if (existing.Status == "Completed")
                {
                    throw new InvalidOperationException("Cannot delete a lab order that has already been processed (Completed).");
                }

                if (existing.IsPaid)
                {
                    throw new InvalidOperationException("Cannot delete a lab order for which payment has been completed.");
                }

                _unitOfWork.Repository<LabOrder>().Remove(existing);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<List<LabOrderListDto>> GetLabOrderListAsync()
        {
            return await _context.LabOrders
                .Select(l => new LabOrderListDto
                {
                    LabOrderId = l.LabOrderId,
                    TestName = l.TestName,
                    Status = l.Status,
                    OrderDate = l.OrderDate,
                    Results = l.Results,
                    AppointmentDate = l.Appointment.AppointmentDate,
                    PatientName = l.Appointment.Patient.FirstName + " " + l.Appointment.Patient.LastName
                })
                .OrderByDescending(l => l.OrderDate)
                .ToListAsync();
        }
    }
}
