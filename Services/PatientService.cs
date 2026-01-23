using BillingSystem.Models;
using BillingSystem.Repositories;

namespace BillingSystem.Services
{
    public interface IPatientService
    {
        Task<int> RegisterPatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task<Patient?> GetPatientDetailsAsync(int id);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByMobileAsync(string mobileNumber);
    }

    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> RegisterPatientAsync(Patient patient)
        {
            // Business Rule: Check Mobile Number Uniqueness
            if (await _unitOfWork.Patients.IsMobileNumberExistsAsync(patient.MobileNumber))
            {
                throw new Exception($"Mobile Number {patient.MobileNumber} already exists.");
            }

            // Business Rule: Senior Citizen Logic
            if (patient.Age >= 60) patient.IsSenior = true;
            else patient.IsSenior = false;

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
    }
}
