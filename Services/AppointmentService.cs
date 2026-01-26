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
                .Include(a => a.Prescriptions)
                    .ThenInclude(p => p.Medicine)
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
            var now = DateTime.Now;
            if (appointment.AppointmentDate.Date < now.Date)
                throw new ArgumentException("Appointment date cannot be in the past.");

            if (appointment.AppointmentDate.Date == now.Date && appointment.AppointmentTime <= now.TimeOfDay)
                throw new ArgumentException("Appointment time has already passed for today.");

            if (!appointment.DoctorId.HasValue)
                throw new ArgumentException("Please select a doctor.");

            var patient = await _context.Patients.FindAsync(appointment.PatientId);
            if (patient == null || !patient.IsActive)
                throw new InvalidOperationException("Cannot schedule appointment: Patient is inactive or not found.");

            var doctor = await _context.Doctors.FindAsync(appointment.DoctorId.Value);
            if (doctor == null || !doctor.IsAvailable)
                throw new InvalidOperationException("Selected doctor is not available.");

            // STRICT DOUBLE-BOOKING CHECK
            var isSlotTaken = await _context.Appointments
                .AnyAsync(a => a.DoctorId == appointment.DoctorId 
                            && a.AppointmentDate.Date == appointment.AppointmentDate.Date 
                            && a.AppointmentTime == appointment.AppointmentTime
                            && a.Status != "Cancelled");

            if (isSlotTaken)
                throw new InvalidOperationException("The selected slot is already taken for this doctor.");

            appointment.CreatedDate = DateTime.Now;
            appointment.Status = "Scheduled";
            appointment.IsPaid = false;

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

        public async Task<List<AppointmentListDto>> GetAppointmentListAsync(int? doctorId = null)
        {
            var query = _context.Appointments.AsQueryable();

            if (doctorId.HasValue && doctorId > 0)
            {
                query = query.Where(a => a.DoctorId == doctorId);
            }

            return await query
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

        public async Task<IEnumerable<TimeSpan>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            var allSlots = GenerateAllSlots();
            
            // Fetch booked appointments for this SPECIFIC doctor and date (exclude Cancelled)
            var bookedTimes = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date && a.Status != "Cancelled")
                .Select(a => a.AppointmentTime)
                .ToListAsync();

            var available = allSlots.Where(s => !bookedTimes.Contains(s)).ToList();

            // If date is today, filter out past slots
            if (date.Date == DateTime.Today)
            {
                var currentTime = DateTime.Now.TimeOfDay;
                available = available.Where(s => s > currentTime).ToList();
            }

            return available;
        }

        private List<TimeSpan> GenerateAllSlots()
        {
            var slots = new List<TimeSpan>();
            var startTime = new TimeSpan(10, 0, 0); // 10 AM
            var endTime = new TimeSpan(17, 0, 0);   // 5 PM (Exclusive of end time, i.e., last appt can start at 4:30 PM)
            var lunchStart = new TimeSpan(13, 0, 0); // 1 PM
            var lunchEnd = new TimeSpan(14, 0, 0);   // 2 PM
            var interval = TimeSpan.FromMinutes(30);

            var currentTime = startTime;
            while (currentTime < endTime)
            {
                // Skip lunch time (1 PM to 2 PM)
                if (currentTime < lunchStart || currentTime >= lunchEnd)
                {
                    slots.Add(currentTime);
                }
                currentTime = currentTime.Add(interval);
            }

            return slots;
        }
    }
}
