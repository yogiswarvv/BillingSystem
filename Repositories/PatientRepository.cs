using BillingSystem.Data;
using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BillingSystem.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<bool> IsMobileNumberExistsAsync(string mobileNumber);
        Task<Patient?> GetPatientWithDetailsAsync(int id);
        Task<int> RegisterPatientSPAsync(Patient patient);
        Task SoftDeletePatientSPAsync(int patientId, string deletedBy);
        Task AdmitPatientSPAsync(int patientId, DateTime admitDate, decimal feePerDay);
        Task<int> CreateAppointmentSPAsync(Appointment appointment);
        Task UpdateAppointmentStatusSPAsync(int appointmentId, string newStatus);
        Task OrderLabTestsSPAsync(int appointmentId, DataTable testOrders);
        Task<int> LogPatientVisitSPAsync(int patientId, int doctorId, DateTime visitDate, string reason);
        Task AddTestsToAppointmentSPAsync(int appointmentId, DataTable testOrders);
        Task<IEnumerable<LabOrder>> SearchLabOrdersSPAsync(string? searchTerm);
        Task UpdateLabResultSPAsync(int labOrderId, string status, string? results, DateTime? completedDate);
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
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.LabOrders)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Prescriptions)
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task<int> RegisterPatientSPAsync(Patient patient)
        {
            var firstName = new SqlParameter("@FirstName", patient.FirstName);
            var lastName = new SqlParameter("@LastName", patient.LastName);
            var dob = new SqlParameter("@DateOfBirth", patient.DateOfBirth);
            var gender = new SqlParameter("@Gender", (int)patient.Gender);
            var mobile = new SqlParameter("@MobileNumber", patient.MobileNumber);
            var email = new SqlParameter("@Email", (object)patient.Email ?? DBNull.Value);
 
             var result = await _context.Database
                .SqlQueryRaw<decimal>("EXEC [Healthcare].[usp_RegisterPatient] @FirstName, @LastName, @DateOfBirth, @Gender, @MobileNumber, @Email", 
                    firstName, lastName, dob, gender, mobile, email)
                .ToListAsync();
            
            return (int)(result.FirstOrDefault());
        }

        public async Task SoftDeletePatientSPAsync(int patientId, string deletedBy)
        {
            var pId = new SqlParameter("@PatientId", patientId);
            var dBy = new SqlParameter("@DeletedBy", deletedBy);
            
            await _context.Database.ExecuteSqlRawAsync("EXEC [Healthcare].[usp_SoftDeletePatient] @PatientId, @DeletedBy", pId, dBy);
        }

        public async Task AdmitPatientSPAsync(int patientId, DateTime admitDate, decimal feePerDay)
        {
            var pId = new SqlParameter("@PatientId", patientId);
            var aDate = new SqlParameter("@AdmitDate", admitDate);
            var fee = new SqlParameter("@FeePerDay", feePerDay);

            await _context.Database.ExecuteSqlRawAsync("EXEC [Healthcare].[usp_AdmitPatient] @PatientId, @AdmitDate, @FeePerDay", pId, aDate, fee);
        }

        public async Task<int> CreateAppointmentSPAsync(Appointment appointment)
        {
             var pId = new SqlParameter("@PatientId", appointment.PatientId);
             var dId = new SqlParameter("@DoctorId", appointment.DoctorId);
             var appDate = new SqlParameter("@AppointmentDate", appointment.AppointmentDate.Date.Add(appointment.AppointmentTime));
             var reason = new SqlParameter("@Reason", (object)appointment.Reason ?? DBNull.Value);

             var result = await _context.Database
                 .SqlQueryRaw<decimal>("EXEC [Healthcare].[usp_CreateAppointment] @PatientId, @DoctorId, @AppointmentDate, @Reason", 
                     pId, dId, appDate, reason)
                 .ToListAsync();

             return (int)(result.FirstOrDefault());
        }

        public async Task UpdateAppointmentStatusSPAsync(int appointmentId, string newStatus)
        {
             var aId = new SqlParameter("@AppointmentId", appointmentId);
             var status = new SqlParameter("@NewStatus", newStatus);

             await _context.Database.ExecuteSqlRawAsync("EXEC [Healthcare].[usp_UpdateAppointmentStatus] @AppointmentId, @NewStatus", aId, status);
        }

        public async Task OrderLabTestsSPAsync(int appointmentId, DataTable testOrders)
        {
            var pId = new SqlParameter("@AppointmentId", appointmentId);
            var tvp = new SqlParameter("@TestOrders", testOrders)
            {
                TypeName = "[Healthcare].[LabTestOrderType]",
                SqlDbType = SqlDbType.Structured
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC [Healthcare].[usp_OrderLabTests] @AppointmentId, @TestOrders", pId, tvp);
        }

        public async Task<int> LogPatientVisitSPAsync(int patientId, int doctorId, DateTime visitDate, string reason)
        {
            var pId = new SqlParameter("@PatientId", patientId);
            var dId = new SqlParameter("@DoctorId", doctorId);
            var vDate = new SqlParameter("@VisitDate", visitDate);
            var rsn = new SqlParameter("@Reason", reason);

            var result = await _context.Database
                .SqlQueryRaw<decimal>("EXEC [Healthcare].[usp_LogPatientVisit] @PatientId, @DoctorId, @VisitDate, @Reason", 
                    pId, dId, vDate, rsn)
                .ToListAsync();

            return (int)(result.FirstOrDefault());
        }

        public async Task AddTestsToAppointmentSPAsync(int appointmentId, DataTable testOrders)
        {
            var pId = new SqlParameter("@AppointmentId", appointmentId);
            var tvp = new SqlParameter("@TestOrders", testOrders)
            {
                TypeName = "[Healthcare].[LabTestOrderType]",
                SqlDbType = SqlDbType.Structured
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC [Healthcare].[usp_AddTestsToAppointment] @AppointmentId, @TestOrders", pId, tvp);
        }

        public async Task<IEnumerable<LabOrder>> SearchLabOrdersSPAsync(string? searchTerm)
        {
            var search = new SqlParameter("@SearchTerm", (object)searchTerm ?? DBNull.Value);

            // Execute SP and map manually or via DTO help
            // Since we need to populate Appointment.Patient for the View, we read flat columns and object graph them.
            
            var labOrders = new List<LabOrder>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC [Healthcare].[usp_SearchLabOrders] @SearchTerm";
                command.Parameters.Add(search);
                _context.Database.OpenConnection();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var order = new LabOrder
                        {
                            LabOrderId = reader.GetInt32(reader.GetOrdinal("LabOrderId")),
                            TestName = reader.GetString(reader.GetOrdinal("TestName")),
                            Cost = reader.IsDBNull(reader.GetOrdinal("Cost")) ? null : reader.GetDecimal(reader.GetOrdinal("Cost")),
                            OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                            Results = reader.IsDBNull(reader.GetOrdinal("Results")) ? null : reader.GetString(reader.GetOrdinal("Results")),
                            IsPaid = reader.GetBoolean(reader.GetOrdinal("IsPaid")),
                            AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                            CompletedDate = reader.IsDBNull(reader.GetOrdinal("CompletedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("CompletedDate")),
                            
                            // Hydrate Navigation Property for View Display
                            Appointment = new Appointment
                            {
                                AppointmentId = reader.GetInt32(reader.GetOrdinal("AppointmentId")),
                                PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                                Patient = new Patient
                                {
                                    PatientId = reader.GetInt32(reader.GetOrdinal("PatientId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName"))
                                }
                            }
                        };
                        labOrders.Add(order);
                    }
                }
            }
            return labOrders;
        }

        public async Task UpdateLabResultSPAsync(int labOrderId, string status, string? results, DateTime? completedDate)
        {
            var lId = new SqlParameter("@LabOrderId", labOrderId);
            var sts = new SqlParameter("@Status", status);
            var res = new SqlParameter("@Results", (object)results ?? DBNull.Value);
            var cDate = new SqlParameter("@CompletedDate", (object)completedDate ?? DBNull.Value);

            await _context.Database.ExecuteSqlRawAsync("EXEC [Healthcare].[usp_UpdateLabResult] @LabOrderId, @Status, @Results, @CompletedDate", 
                lId, sts, res, cDate);
        }
    }
}
