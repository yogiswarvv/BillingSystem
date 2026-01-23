using BillingSystem.Data;
using BillingSystem.Models;
using BillingSystem.Repositories;
using BillingSystem.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace BillingSystem.Services
{
    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HospitalDbContext _context;

        public AppointmentService(IUnitOfWork unitOfWork, HospitalDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.LabOrders)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.LabOrders)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.LabOrders)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            if (appointment.AppointmentDate.Date < DateTime.Now.Date)
                throw new ArgumentException("Appointment date cannot be in the past.");

            if (appointment.DoctorId.HasValue)
            {
                var isAvailable = await _context.Doctors
                    .AnyAsync(d => d.DoctorId == appointment.DoctorId && d.IsAvailable);

                if (!isAvailable)
                    throw new InvalidOperationException("Selected doctor is not available.");
            }

            appointment.CreatedDate = DateTime.Now;
            appointment.Status = "Scheduled";

            await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
            await _unitOfWork.CompleteAsync();
            return appointment;
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            var existing = await _unitOfWork.Repository<Appointment>().GetByIdAsync(appointment.AppointmentId);
            if (existing == null)
                throw new InvalidOperationException($"Appointment with ID {appointment.AppointmentId} not found.");

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAppointmentAsync(int id)
        {
            var existing = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
            if (existing != null)
            {
                _unitOfWork.Repository<Appointment>().Remove(existing);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<List<AppointmentListDto>> GetAppointmentListAsync()
        {
            return await _context.Appointments
                .Select(a => new AppointmentListDto
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    DoctorName = a.DoctorName,
                    Reason = a.Reason,
                    Status = a.Status,
                    PatientName = a.Patient.FirstName + " " + a.Patient.LastName,
                    LabOrderCount = a.LabOrders.Count()
                })
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<List<LabOrder>> GetLabOrdersByAppointmentIdAsync(int appointmentId)
        {
            return await _context.LabOrders
                .Where(l => l.AppointmentId == appointmentId)
                .ToListAsync();
        }
    }
}
