using BillingSystem.Data;
using BillingSystem.Models;
using BillingSystem.Repositories;
using BillingSystem.DTOs;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<LabOrder>> GetAllLabOrdersAsync()
        {
            return await _context.LabOrders
                .Include(l => l.Appointment)
                    .ThenInclude(a => a.Patient)
                .OrderByDescending(l => l.OrderDate)
                .ToListAsync();
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

        public async Task<LabOrder> CreateLabOrderAsync(LabOrder labOrder)
        {
            if (string.IsNullOrWhiteSpace(labOrder.TestName))
                throw new ArgumentException("Test name is required.");

            var appointment = await _context.Appointments.Include(a => a.Patient).FirstOrDefaultAsync(a => a.AppointmentId == labOrder.AppointmentId);
            if (appointment == null)
                throw new InvalidOperationException($"Appointment with ID {labOrder.AppointmentId} not found.");

            // Rule: Duplicate Lab Order Prevention (Appointment Level)
            var isDuplicateForAppointment = await _context.LabOrders
                .AnyAsync(l => l.AppointmentId == labOrder.AppointmentId && 
                               l.TestName == labOrder.TestName);

            if (isDuplicateForAppointment)
            {
                throw new InvalidOperationException($"A lab order for '{labOrder.TestName}' has already been placed for this appointment.");
            }

            // Rule: Duplicate ECG Prevention (Patient Level - Only if already Completed)
            if (labOrder.TestName.Equals("ECG", StringComparison.OrdinalIgnoreCase))
            {
                var hasCompletedECG = await _context.LabOrders
                    .AnyAsync(l => l.Appointment.PatientId == appointment.PatientId && 
                                   l.TestName == "ECG" && 
                                   l.Status == "Completed");
                
                if (hasCompletedECG)
                {
                    throw new InvalidOperationException("This patient has already completed an ECG. Duplicate ECG orders are not allowed.");
                }
            }

            labOrder.Appointment = appointment;
            labOrder.OrderDate = DateTime.Now;
            labOrder.Status = "Pending";
            
            await _unitOfWork.Repository<LabOrder>().AddAsync(labOrder);
            await _unitOfWork.CompleteAsync();
            return labOrder;
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
            var labOrder = await _unitOfWork.Repository<LabOrder>().GetByIdAsync(labOrderId);
            if (labOrder == null)
                throw new InvalidOperationException($"Lab order with ID {labOrderId} not found.");

            labOrder.Status = status;
            labOrder.Results = results;

            if (status == "Completed")
            {
                labOrder.CompletedDate = DateTime.Now;
            }

            _unitOfWork.Repository<LabOrder>().Update(labOrder);
            await _unitOfWork.CompleteAsync();
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
