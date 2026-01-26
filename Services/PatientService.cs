using BillingSystem.Models;
using BillingSystem.Repositories;
using BillingSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Services
{
    public interface IPatientService
    {
        Task<int> RegisterPatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task<Patient?> GetPatientDetailsAsync(int id);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<IEnumerable<Patient>> GetAllPatientsIncludeInactiveAsync();
        Task<Patient?> GetPatientByMobileAsync(string mobileNumber);
        Task DeletePatientAsync(int patientId, string? deletedBy = null);
        Task ActivatePatientAsync(int patientId);
    }

    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HospitalDbContext _context;

        public PatientService(IUnitOfWork unitOfWork, HospitalDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<int> RegisterPatientAsync(Patient patient)
        {
            // Business Rule: Check Mobile Number Uniqueness
            if (await _unitOfWork.Patients.IsMobileNumberExistsAsync(patient.MobileNumber))
            {
                throw new Exception($"Mobile Number {patient.MobileNumber} already exists.");
            }

            // Senior Citizen logic is now handled automatically by the Patient model age calculation.

            patient.CreatedDate = DateTime.Now;
            patient.IsActive = true;

            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.CompleteAsync();

            return patient.PatientId;
        }

        public async Task UpdatePatientAsync(Patient patient)
        {
             // Validate if exists
             var existing = await _unitOfWork.Patients.GetByIdAsync(patient.PatientId);
             if (existing == null) throw new Exception("Patient not found");

             _unitOfWork.Patients.Update(patient);
             await _unitOfWork.CompleteAsync();
        }

        public async Task<Patient?> GetPatientDetailsAsync(int id)
        {
            return await _unitOfWork.Patients.GetPatientWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _unitOfWork.Patients.FindAsync(p => p.IsActive);
        }

        public async Task<IEnumerable<Patient>> GetAllPatientsIncludeInactiveAsync()
        {
            return await _unitOfWork.Patients.GetAllAsync();
        }

        public async Task<Patient?> GetPatientByMobileAsync(string mobileNumber)
        {
             // Assuming Repository has a find method or using FindAsync
             // If bespoke method needed, might need to add to repository interface too.
             // But existing FindAsync predicate is sufficient for now if Exposed via generic repo logic or just use explicit Find
             var patients = await _unitOfWork.Patients.FindAsync(p => p.MobileNumber == mobileNumber);
             return patients.FirstOrDefault();
        }

        public async Task DeletePatientAsync(int patientId, string? deletedBy = null)
        {
            var patient = await _context.Patients
                .Include(p => p.Appointments)
                .Include(p => p.Bills)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null) throw new Exception("Patient not found.");

            // Rule: Appointment-Based Deletion Restriction
            if (patient.Appointments.Any(a => a.Status == "Scheduled" || a.Status == "Active"))
            {
                throw new InvalidOperationException("Cannot delete patient: There are scheduled or active appointments. All appointments must be completed first.");
            }

            // Rule: Patient Deletion Rules (Lab Orders)
            var hasActiveLabOrders = await _context.LabOrders.AnyAsync(l => l.Appointment.PatientId == patientId && (l.Status == "Pending" || l.Status == "Processing"));
            if (hasActiveLabOrders)
            {
                throw new InvalidOperationException("Active Lab Order: Cannot delete patient because pending or processing lab orders exist.");
            }

            // Rule: Patient Deletion Rules (Payments)
            if (patient.Bills.Any(b => b.Status == BillStatus.Unpaid))
            {
                throw new InvalidOperationException("Bill Not Paid: Cannot delete patient because pending payments exist.");
            }

            // Rule: Record Deletion Handling
            var deletedPatientRecord = new PastRecordPatient
            {
                OriginalPatientId = patient.PatientId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                MobileNumber = patient.MobileNumber,
                Email = patient.Email,
                Address = patient.Address,
                CreatedDate = patient.CreatedDate,
                ArchivedDate = DateTime.Now,
                ArchivedBy = deletedBy ?? "SYSTEM"
            };

            await _context.PastRecordPatients.AddAsync(deletedPatientRecord);
            patient.IsActive = false;
            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.CompleteAsync();
        }

        public async Task ActivatePatientAsync(int patientId)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
            if (patient == null) throw new Exception("Patient not found.");

            patient.IsActive = true;
            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.CompleteAsync();
        }
    }
}
