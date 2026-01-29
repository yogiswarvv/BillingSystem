using BillingSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
        {
        }

        public DbSet<IdentityUser> Users { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityUserRole<string>> UserRoles { get; set; }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<LabOrder> LabOrders { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<ServiceMaster> Services { get; set; }
        public DbSet<PatientService> PatientServices { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItem> BillItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        // Production Insurance Module (Federated Registries)
        public DbSet<InsuranceProvider> InsuranceProviders { get; set; }
        public DbSet<InsurancePlan> InsurancePlans { get; set; }
        public DbSet<ApolloMunichRegistry> ApolloMunichRegistry { get; set; }
        public DbSet<HDFCErgoRegistry> HDFCErgoRegistry { get; set; }
        public DbSet<StarHealthRegistry> StarHealthRegistry { get; set; }
        public DbSet<InsuranceClaim> InsuranceClaims { get; set; }

        public DbSet<PastRecordPatient> PastRecordPatients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Schema
            modelBuilder.HasDefaultSchema("Healthcare");

            // Identity Minimal Configuration
            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.ToTable("AspNetUsers", "Healthcare");
                entity.HasKey(u => u.Id);
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("AspNetRoles", "Healthcare");
                entity.HasKey(r => r.Id);
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AspNetUserRoles", "Healthcare");
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            });

            // --- FLUENT API CONFIGURATION ---

            // 1. Patient
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patients", "Healthcare", tb => tb.HasTrigger("trg_AuditPatients"));
                entity.HasIndex(p => p.MobileNumber).HasDatabaseName("IX_Patient_MobileNumber");
                entity.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.LastName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.MobileNumber).IsRequired().HasMaxLength(10);
                entity.Property(p => p.CreatedDate).HasDefaultValueSql("GETDATE()");
                
                // One-to-many with Appointments
                entity.HasMany(p => p.Appointments)
                      .WithOne(a => a.Patient)
                      .HasForeignKey(a => a.PatientId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 2. Appointment
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointment", "Healthcare", tb => tb.HasTrigger("trg_AuditAppointment"));
                entity.HasKey(a => a.AppointmentId);
                entity.Property(a => a.Status).HasDefaultValue("Scheduled");
                entity.Property(a => a.CreatedDate).HasDefaultValueSql("GETDATE()");
                
                // One-to-many with LabOrders
                entity.HasMany(a => a.LabOrders)
                      .WithOne(l => l.Appointment)
                      .HasForeignKey(l => l.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 3. Doctor
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctor", "Healthcare", tb => tb.HasTrigger("trg_AuditDoctor"));
                entity.HasKey(d => d.DoctorId);
                entity.Property(d => d.CreatedDate).HasDefaultValueSql("GETDATE()");
            });

            // Configure Appointment <-> Doctor
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. LabOrder
            modelBuilder.Entity<LabOrder>(entity =>
            {
                entity.ToTable("LabOrder", "Healthcare", tb => tb.HasTrigger("trg_AuditLabOrder"));
                entity.HasKey(l => l.LabOrderId);
                entity.Property(l => l.Status).HasDefaultValue("Pending");
                entity.Property(l => l.OrderDate).HasDefaultValueSql("GETDATE()");
            });

            // 5. AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLog", "Healthcare", tb => tb.HasTrigger("trg_ProtectAuditLog"));
                entity.Property(a => a.ChangedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(a => a.ChangedBy).HasDefaultValue("SYSTEM");
            });

            // 6. ServiceMaster
            modelBuilder.Entity<ServiceMaster>(entity =>
            {
                entity.ToTable("ServicesMaster", "Healthcare", tb => tb.HasTrigger("trg_AuditServicesMaster"));
                entity.HasIndex(s => s.ServiceName).IsUnique().HasDatabaseName("IX_Service_Name");
                entity.Property(s => s.Cost).HasColumnType("decimal(18,2)");
                entity.Property(s => s.CreatedDate).HasDefaultValueSql("GETDATE()");
            });

            // 3. Bill & BillItem & Payment (Money Precision)
            modelBuilder.Entity<Bill>(entity => 
            {
                entity.ToTable("Bills", "Healthcare", tb => tb.HasTrigger("trg_AuditBills"));
                entity.HasIndex(b => b.BillDate).HasDatabaseName("IX_Bill_BillDate");
                entity.Property(b => b.ConsultationFee).HasColumnType("decimal(18,2)").HasDefaultValue(500m);
                entity.Property(b => b.OptionalServicesAmount).HasColumnType("decimal(18,2)");
                entity.Property(b => b.AdmitAmount).HasColumnType("decimal(18,2)");
                entity.Property(b => b.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(b => b.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(b => b.SeniorDiscount).HasColumnType("decimal(18,2)");
                entity.Property(b => b.InsuranceDeduction).HasColumnType("decimal(18,2)");
                entity.Property(b => b.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(b => b.FinalAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<BillItem>().Property(bi => bi.Amount).HasColumnType("decimal(18,2)");
            
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments", "Healthcare", tb => tb.HasTrigger("trg_AuditPayments"));
                entity.HasIndex(p => p.TransactionRef).HasDatabaseName("IX_Payment_TransactionRef");
                entity.Property(p => p.PaidAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Admission>(entity =>
            {
                entity.ToTable("Admissions", "Healthcare", tb => tb.HasTrigger("trg_AuditAdmissions"));
                entity.Property(a => a.FeePerDay).HasColumnType("decimal(18,2)").HasDefaultValue(2000m);
            });

            // --- RELATIONSHIPS (ON DELETE RESTRICT) ---
            
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Patient)
                .WithMany(p => p.Bills)
                .HasForeignKey(b => b.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Bill)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BillItem>()
                .HasOne(bi => bi.Bill)
                .WithMany(b => b.BillItems)
                .HasForeignKey(bi => bi.BillId)
                .OnDelete(DeleteBehavior.Cascade); // Items deleted if Bill is (soft delete preferred usually, but Cascade ok for Items)

            modelBuilder.Entity<PatientService>()
                .HasOne(ps => ps.Patient)
                .WithMany(p => p.PatientServices)
                .HasForeignKey(ps => ps.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Admission>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Admissions)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Insurance>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Insurances)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- SEED DATA ---
            modelBuilder.Entity<Patient>().HasData(
                new Patient { PatientId = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1980, 1, 1), Gender = Gender.Male, MobileNumber = "9876543210", IsActive = true, CreatedDate = new DateTime(2024, 1, 1) },
                new Patient { PatientId = 2, FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateTime(1960, 1, 1), Gender = Gender.Female, MobileNumber = "8765432109", IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }
            );

            modelBuilder.Entity<ServiceMaster>().HasData(
                new ServiceMaster { ServiceId = 1, ServiceName = "Lab Test", ServiceCode = "LAB001", Cost = 600, IsActive = true, Department = "Pathology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 2, ServiceName = "X-Ray", ServiceCode = "RAD001", Cost = 300, IsActive = true, Department = "Radiology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 3, ServiceName = "ECG", ServiceCode = "CRD001", Cost = 1500, IsActive = true, Department = "Cardiology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 4, ServiceName = "MRI", ServiceCode = "RAD002", Cost = 2500, IsActive = true, Department = "Radiology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 5, ServiceName = "Ultrasound", ServiceCode = "RAD003", Cost = 1200, IsActive = true, Department = "Radiology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 6, ServiceName = "Blood Work", ServiceCode = "LAB002", Cost = 400, IsActive = true, Department = "Pathology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 7, ServiceName = "Lipid Profile", ServiceCode = "LAB007", Cost = 800, IsActive = true, Department = "Pathology", CreatedDate = new DateTime(2024, 1, 1) },
                #pragma warning disable CA1861 // Avoid constant arrays as arguments
                new ServiceMaster { ServiceId = 8, ServiceName = "Kidney Function Test", ServiceCode = "LAB008", Cost = 950, IsActive = true, Department = "Pathology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 9, ServiceName = "Thyroid Profile", ServiceCode = "LAB009", Cost = 1100, IsActive = true, Department = "Pathology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 10, ServiceName = "Blood Sugar", ServiceCode = "LAB010", Cost = 100, IsActive = true, Department = "Pathology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 11, ServiceName = "Liver Function Test", ServiceCode = "LAB011", Cost = 1200, IsActive = true, Department = "Pathology", CreatedDate = new DateTime(2024, 1, 1) }
            );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { DoctorId = 1, FirstName = "Aditya", LastName = "Verma", Specialization = "Cardiology", IsAvailable = true, CreatedDate = new DateTime(2024, 1, 1) },
                new Doctor { DoctorId = 2, FirstName = "Sriya", LastName = "Reddy", Specialization = "Diagnostics", IsAvailable = true, CreatedDate = new DateTime(2024, 1, 1) },
                new Doctor { DoctorId = 3, FirstName = "Vikram", LastName = "Singh", Specialization = "Radiology", IsAvailable = true, CreatedDate = new DateTime(2024, 1, 1) }
            );

            // 7. Pharmacy Seeding
            modelBuilder.Entity<Medicine>(entity => entity.ToTable("Medicines", "Healthcare", tb => tb.HasTrigger("trg_AuditMedicines")));
            modelBuilder.Entity<Prescription>(entity => entity.ToTable("Prescriptions", "Healthcare", tb => tb.HasTrigger("trg_AuditPrescriptions")));
            modelBuilder.Entity<InsuranceProvider>(entity => entity.ToTable("InsuranceProviders", "Healthcare", tb => tb.HasTrigger("trg_AuditInsuranceProviders")));

            modelBuilder.Entity<Medicine>().HasData(
                new Medicine { MedicineId = 1, Name = "Paracetamol", DosageStrength = "500mg", Category = "Analgesic", PricePerUnit = 2, Stock = 1000, ExpiryDate = new DateTime(2026, 12, 31) },
                new Medicine { MedicineId = 2, Name = "Amoxicillin", DosageStrength = "250mg", Category = "Antibiotic", PricePerUnit = 15, Stock = 500, ExpiryDate = new DateTime(2025, 6, 30) },
                new Medicine { MedicineId = 3, Name = "Cetirizine", DosageStrength = "10mg", Category = "Antihistamine", PricePerUnit = 5, Stock = 800, ExpiryDate = new DateTime(2026, 3, 1) },
                new Medicine { MedicineId = 4, Name = "Metformin", DosageStrength = "500mg", Category = "Antidiabetic", PricePerUnit = 8, Stock = 1200, ExpiryDate = new DateTime(2027, 1, 1) },
                new Medicine { MedicineId = 5, Name = "Atorvastatin", DosageStrength = "20mg", Category = "Statin", PricePerUnit = 25, Stock = 300, ExpiryDate = new DateTime(2025, 12, 31) },
                new Medicine { MedicineId = 6, Name = "Omeprazole", DosageStrength = "20mg", Category = "Proton Pump Inhibitor", PricePerUnit = 12, Stock = 500, ExpiryDate = new DateTime(2026, 8, 15) },
                new Medicine { MedicineId = 7, Name = "Amlodipine", DosageStrength = "5mg", Category = "Bloop Pressure", PricePerUnit = 10, Stock = 600, ExpiryDate = new DateTime(2026, 5, 20) },
                new Medicine { MedicineId = 8, Name = "Ibuprofen", DosageStrength = "400mg", Category = "Analgesic", PricePerUnit = 3, Stock = 900, ExpiryDate = new DateTime(2026, 11, 10) },
                new Medicine { MedicineId = 9, Name = "Azithromycin", DosageStrength = "500mg", Category = "Antibiotic", PricePerUnit = 45, Stock = 200, ExpiryDate = new DateTime(2025, 4, 1) },
                new Medicine { MedicineId = 10, Name = "Metoprolol", DosageStrength = "500mg", Category = "Beta Blocker", PricePerUnit = 18, Stock = 400, ExpiryDate = new DateTime(2026, 2, 28) },
                new Medicine { MedicineId = 11, Name = "Losartan", DosageStrength = "500mg", Category = "Antihypertensive", PricePerUnit = 22, Stock = 350, ExpiryDate = new DateTime(2026, 7, 1) },
                new Medicine { MedicineId = 12, Name = "Gabapentin", DosageStrength = "300mg", Category = "Anticonvulsant", PricePerUnit = 35, Stock = 250, ExpiryDate = new DateTime(2025, 9, 30) },
                new Medicine { MedicineId = 13, Name = "Sertraline", DosageStrength = "50mg", Category = "Antidepressant", PricePerUnit = 40, Stock = 150, ExpiryDate = new DateTime(2025, 11, 1) },
                new Medicine { MedicineId = 14, Name = "Pantoprazole", DosageStrength = "40mg", Category = "Proton Pump Inhibitor", PricePerUnit = 14, Stock = 500, ExpiryDate = new DateTime(2026, 12, 1) },
                new Medicine { MedicineId = 15, Name = "Aspirin", DosageStrength = "75mg", Category = "Antiplatelet", PricePerUnit = 1, Stock = 2000, ExpiryDate = new DateTime(2027, 6, 1) },
                new Medicine { MedicineId = 16, Name = "Prednisone", DosageStrength = "5mg", Category = "Corticosteroid", PricePerUnit = 20, Stock = 300, ExpiryDate = new DateTime(2025, 3, 15) },
                new Medicine { MedicineId = 17, Name = "Salbutamol", DosageStrength = "100mcg", Category = "Bronchodilator", PricePerUnit = 150, Stock = 100, ExpiryDate = new DateTime(2025, 8, 1) },
                new Medicine { MedicineId = 18, Name = "Furosemide", DosageStrength = "40mg", Category = "Diuretic", PricePerUnit = 6, Stock = 500, ExpiryDate = new DateTime(2026, 1, 1) },
                new Medicine { MedicineId = 19, Name = "Insulin Glargine", DosageStrength = "100U/ml", Category = "Insulin", PricePerUnit = 800, Stock = 50, ExpiryDate = new DateTime(2025, 5, 1) },
                new Medicine { MedicineId = 20, Name = "Ciprofloxacin", DosageStrength = "500mg", Category = "Antibiotic", PricePerUnit = 30, Stock = 400, ExpiryDate = new DateTime(2025, 10, 1) }
            );

            modelBuilder.Entity<Insurance>().HasData(
                new Insurance { InsuranceId = 1, PatientId = 1, ProviderName = "Apollo Munich", PolicyNumber = "AM-1001", CoveragePercent = 80, CoverageType = InsuranceCoverageType.FullBill, IsActive = true },
                new Insurance { InsuranceId = 2, PatientId = 1, ProviderName = "Star Health", PolicyNumber = "SH-3001", CoveragePercent = 50, CoverageType = InsuranceCoverageType.OptionalServicesOnly, IsActive = true },
                new Insurance { InsuranceId = 3, PatientId = 2, ProviderName = "HDFC Ergo", PolicyNumber = "HE-2001", CoveragePercent = 100, CoverageType = InsuranceCoverageType.AdmitFeeOnly, IsActive = true }
            );

            // --- PRODUCTION INSURANCE SEEDING (60 Records) ---
            modelBuilder.Entity<InsuranceProvider>().HasData(
                new InsuranceProvider { ProviderID = 1, ProviderName = "Apollo Munich", ContactEmail = "claims@apollomunich.in", TollFreeNumber = "1800-123-4444" },
                new InsuranceProvider { ProviderID = 2, ProviderName = "HDFC Ergo", ContactEmail = "support@hdfcergo.com", TollFreeNumber = "1800-222-3333" },
                new InsuranceProvider { ProviderID = 3, ProviderName = "Star Health", ContactEmail = "info@starhealth.in", TollFreeNumber = "1800-555-6666" }
            );

            modelBuilder.Entity<InsurancePlan>().HasData(
                new InsurancePlan { PlanID = 1, ProviderID = 1, PlanName = "Silver Elite", CoveragePercentage = 80.00m, MaxBenefitPerClaim = 50000 },
                new InsurancePlan { PlanID = 2, ProviderID = 1, PlanName = "Gold Shield", CoveragePercentage = 100.00m, MaxBenefitPerClaim = 150000 },
                new InsurancePlan { PlanID = 3, ProviderID = 2, PlanName = "Easy Health Plus", CoveragePercentage = 90.00m, MaxBenefitPerClaim = 80000 },
                new InsurancePlan { PlanID = 4, ProviderID = 2, PlanName = "Optima Premium", CoveragePercentage = 100.00m, MaxBenefitPerClaim = 500000 },
                new InsurancePlan { PlanID = 5, ProviderID = 3, PlanName = "Senior Red Carpet", CoveragePercentage = 70.00m, MaxBenefitPerClaim = 40000 },
                new InsurancePlan { PlanID = 6, ProviderID = 3, PlanName = "Family Pro", CoveragePercentage = 100.00m, MaxBenefitPerClaim = 200000 }
            );

            // 8. Insurance Member Registry (Restrict Delete)
            modelBuilder.Entity<InsuranceMemberRegistry>(entity =>
            {
                entity.HasOne(m => m.Provider)
                      .WithMany()
                      .HasForeignKey(m => m.ProviderID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Plan)
                      .WithMany()
                      .HasForeignKey(m => m.PlanID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // AM-1001 to AM-1020
            modelBuilder.Entity<InsuranceMemberRegistry>().HasData(
                new InsuranceMemberRegistry { MemberID = 1, PolicyNumber = "AM-1001", ProviderID = 1, PlanID = 1, FullName = "Aarav Gupta", DateOfBirth = new DateTime(1985, 5, 10), RemainingBalance = 45000 },
                new InsuranceMemberRegistry { MemberID = 2, PolicyNumber = "AM-1002", ProviderID = 1, PlanID = 1, FullName = "Ishani Singh", DateOfBirth = new DateTime(1990, 12, 15), RemainingBalance = 38000 },
                new InsuranceMemberRegistry { MemberID = 3, PolicyNumber = "AM-1003", ProviderID = 1, PlanID = 2, FullName = "Vihaan Sharma", DateOfBirth = new DateTime(1975, 3, 22), RemainingBalance = 120000 },
                new InsuranceMemberRegistry { MemberID = 4, PolicyNumber = "AM-1004", ProviderID = 1, PlanID = 2, FullName = "Myra Kapoor", DateOfBirth = new DateTime(1988, 8, 30), RemainingBalance = 95000 },
                new InsuranceMemberRegistry { MemberID = 5, PolicyNumber = "AM-1005", ProviderID = 1, PlanID = 1, FullName = "Reyansh Reddy", DateOfBirth = new DateTime(1995, 1, 12), RemainingBalance = 48000 },
                new InsuranceMemberRegistry { MemberID = 6, PolicyNumber = "AM-1006", ProviderID = 1, PlanID = 2, FullName = "Anaya Jain", DateOfBirth = new DateTime(1982, 6, 5), RemainingBalance = 135000 },
                new InsuranceMemberRegistry { MemberID = 7, PolicyNumber = "AM-1007", ProviderID = 1, PlanID = 1, FullName = "Sai Kumar", DateOfBirth = new DateTime(1968, 11, 20), RemainingBalance = 22000 },
                new InsuranceMemberRegistry { MemberID = 8, PolicyNumber = "AM-1008", ProviderID = 1, PlanID = 2, FullName = "Diya Malhotra", DateOfBirth = new DateTime(1993, 2, 28), RemainingBalance = 110000 },
                new InsuranceMemberRegistry { MemberID = 9, PolicyNumber = "AM-1009", ProviderID = 1, PlanID = 1, FullName = "Advait Bhosale", DateOfBirth = new DateTime(1970, 7, 14), RemainingBalance = 31000 },
                new InsuranceMemberRegistry { MemberID = 10, PolicyNumber = "AM-1010", ProviderID = 1, PlanID = 2, FullName = "Saanvi Iyer", DateOfBirth = new DateTime(1987, 4, 19), RemainingBalance = 88000 },
                new InsuranceMemberRegistry { MemberID = 11, PolicyNumber = "AM-1011", ProviderID = 1, PlanID = 1, FullName = "Arjun Pillai", DateOfBirth = new DateTime(1981, 10, 9), RemainingBalance = 42000 },
                new InsuranceMemberRegistry { MemberID = 12, PolicyNumber = "AM-1012", ProviderID = 1, PlanID = 2, FullName = "Kavya Nair", DateOfBirth = new DateTime(1996, 3, 3), RemainingBalance = 145000 },
                new InsuranceMemberRegistry { MemberID = 13, PolicyNumber = "AM-1013", ProviderID = 1, PlanID = 1, FullName = "Rudra Pandey", DateOfBirth = new DateTime(1973, 9, 21), RemainingBalance = 15000 },
                new InsuranceMemberRegistry { MemberID = 14, PolicyNumber = "AM-1014", ProviderID = 1, PlanID = 2, FullName = "Anika Gupta", DateOfBirth = new DateTime(1984, 12, 11), RemainingBalance = 102000 },
                new InsuranceMemberRegistry { MemberID = 15, PolicyNumber = "AM-1015", ProviderID = 1, PlanID = 1, FullName = "Vivaan Joshi", DateOfBirth = new DateTime(1991, 5, 31), RemainingBalance = 49000 },
                new InsuranceMemberRegistry { MemberID = 16, PolicyNumber = "AM-1016", ProviderID = 1, PlanID = 2, FullName = "Aira Saxena", DateOfBirth = new DateTime(1989, 2, 22), RemainingBalance = 121000 },
                new InsuranceMemberRegistry { MemberID = 17, PolicyNumber = "AM-1017", ProviderID = 1, PlanID = 1, FullName = "Kabir Mehta", DateOfBirth = new DateTime(1965, 8, 8), RemainingBalance = 8000 },
                new InsuranceMemberRegistry { MemberID = 18, PolicyNumber = "AM-1018", ProviderID = 1, PlanID = 2, FullName = "Vanya Chopra", DateOfBirth = new DateTime(1998, 11, 14), RemainingBalance = 150000 },
                new InsuranceMemberRegistry { MemberID = 19, PolicyNumber = "AM-1019", ProviderID = 1, PlanID = 1, FullName = "Dhruv Agrawal", DateOfBirth = new DateTime(1977, 6, 25), RemainingBalance = 29000 },
                new InsuranceMemberRegistry { MemberID = 20, PolicyNumber = "AM-1020", ProviderID = 1, PlanID = 2, FullName = "Kiara Das", DateOfBirth = new DateTime(1986, 7, 17), RemainingBalance = 115000 },

                // HE-2001 to HE-2020
                new InsuranceMemberRegistry { MemberID = 21, PolicyNumber = "HE-2001", ProviderID = 2, PlanID = 3, FullName = "Suresh Mani", DateOfBirth = new DateTime(1965, 11, 30), RemainingBalance = 70000 },
                new InsuranceMemberRegistry { MemberID = 22, PolicyNumber = "HE-2002", ProviderID = 2, PlanID = 3, FullName = "Ananya Roy", DateOfBirth = new DateTime(1992, 4, 15), RemainingBalance = 65000 },
                new InsuranceMemberRegistry { MemberID = 23, PolicyNumber = "HE-2003", ProviderID = 2, PlanID = 4, FullName = "Rahul Dravid", DateOfBirth = new DateTime(1980, 1, 11), RemainingBalance = 450000 },
                new InsuranceMemberRegistry { MemberID = 24, PolicyNumber = "HE-2004", ProviderID = 2, PlanID = 4, FullName = "Meera Bai", DateOfBirth = new DateTime(1988, 6, 21), RemainingBalance = 480000 },
                new InsuranceMemberRegistry { MemberID = 25, PolicyNumber = "HE-2005", ProviderID = 2, PlanID = 3, FullName = "Siddharth Roy", DateOfBirth = new DateTime(1994, 9, 12), RemainingBalance = 78000 },
                new InsuranceMemberRegistry { MemberID = 26, PolicyNumber = "HE-2006", ProviderID = 2, PlanID = 4, FullName = "Pooja Bhatt", DateOfBirth = new DateTime(1985, 5, 15), RemainingBalance = 300000 },
                new InsuranceMemberRegistry { MemberID = 27, PolicyNumber = "HE-2007", ProviderID = 2, PlanID = 3, FullName = "Amitabh Bach", DateOfBirth = new DateTime(1955, 10, 10), RemainingBalance = 32000 },
                new InsuranceMemberRegistry { MemberID = 28, PolicyNumber = "HE-2008", ProviderID = 2, PlanID = 4, FullName = "Salman Khan", DateOfBirth = new DateTime(1972, 12, 27), RemainingBalance = 420000 },
                new InsuranceMemberRegistry { MemberID = 29, PolicyNumber = "HE-2009", ProviderID = 2, PlanID = 3, FullName = "Arun Jaitley", DateOfBirth = new DateTime(1960, 3, 24), RemainingBalance = 55000 },
                new InsuranceMemberRegistry { MemberID = 30, PolicyNumber = "HE-2010", ProviderID = 2, PlanID = 4, FullName = "Smriti Irani", DateOfBirth = new DateTime(1982, 2, 18), RemainingBalance = 350000 },
                new InsuranceMemberRegistry { MemberID = 31, PolicyNumber = "HE-2011", ProviderID = 2, PlanID = 3, FullName = "Kapil Dev", DateOfBirth = new DateTime(1967, 8, 20), RemainingBalance = 68000 },
                new InsuranceMemberRegistry { MemberID = 32, PolicyNumber = "HE-2012", ProviderID = 2, PlanID = 4, FullName = "Sunil Gavak", DateOfBirth = new DateTime(1975, 4, 14), RemainingBalance = 490000 },
                new InsuranceMemberRegistry { MemberID = 33, PolicyNumber = "HE-2013", ProviderID = 2, PlanID = 3, FullName = "Virender Seh", DateOfBirth = new DateTime(1983, 11, 1), RemainingBalance = 45000 },
                new InsuranceMemberRegistry { MemberID = 34, PolicyNumber = "HE-2014", ProviderID = 2, PlanID = 4, FullName = "Sachin Ram", DateOfBirth = new DateTime(1981, 4, 24), RemainingBalance = 400000 },
                new InsuranceMemberRegistry { MemberID = 35, PolicyNumber = "HE-2015", ProviderID = 2, PlanID = 3, FullName = "Rohit Shar", DateOfBirth = new DateTime(1990, 7, 12), RemainingBalance = 72000 },
                new InsuranceMemberRegistry { MemberID = 36, PolicyNumber = "HE-2016", ProviderID = 2, PlanID = 4, FullName = "Shikhar Dhav", DateOfBirth = new DateTime(1987, 12, 5), RemainingBalance = 380000 },
                new InsuranceMemberRegistry { MemberID = 37, PolicyNumber = "HE-2017", ProviderID = 2, PlanID = 3, FullName = "M.S. Dhoni", DateOfBirth = new DateTime(1985, 7, 7), RemainingBalance = 85000 },
                new InsuranceMemberRegistry { MemberID = 38, PolicyNumber = "HE-2018", ProviderID = 2, PlanID = 4, FullName = "Yuvraj Singh", DateOfBirth = new DateTime(1986, 12, 12), RemainingBalance = 450000 },
                new InsuranceMemberRegistry { MemberID = 39, PolicyNumber = "HE-2019", ProviderID = 2, PlanID = 3, FullName = "Jasprit Bum", DateOfBirth = new DateTime(1995, 2, 10), RemainingBalance = 50000 },
                new InsuranceMemberRegistry { MemberID = 40, PolicyNumber = "HE-2020", ProviderID = 2, PlanID = 4, FullName = "Virat Kohli", DateOfBirth = new DateTime(1990, 11, 5), RemainingBalance = 500000 },

                // SH-3001 to SH-3020
                new InsuranceMemberRegistry { MemberID = 41, PolicyNumber = "SH-3001", ProviderID = 3, PlanID = 5, FullName = "Lata Mangesh", DateOfBirth = new DateTime(1945, 9, 20), RemainingBalance = 5000 },
                new InsuranceMemberRegistry { MemberID = 42, PolicyNumber = "SH-3002", ProviderID = 3, PlanID = 5, FullName = "Mukesh Amb", DateOfBirth = new DateTime(1955, 4, 19), RemainingBalance = 35000 },
                new InsuranceMemberRegistry { MemberID = 43, PolicyNumber = "SH-3003", ProviderID = 3, PlanID = 6, FullName = "Ratan Tata", DateOfBirth = new DateTime(1940, 12, 28), RemainingBalance = 180000 },
                new InsuranceMemberRegistry { MemberID = 44, PolicyNumber = "SH-3004", ProviderID = 3, PlanID = 6, FullName = "Azim Premji", DateOfBirth = new DateTime(1948, 7, 24), RemainingBalance = 150000 },
                new InsuranceMemberRegistry { MemberID = 45, PolicyNumber = "SH-3005", ProviderID = 3, PlanID = 5, FullName = "Kiran Mazum", DateOfBirth = new DateTime(1960, 3, 23), RemainingBalance = 25000 },
                new InsuranceMemberRegistry { MemberID = 46, PolicyNumber = "SH-3006", ProviderID = 3, PlanID = 6, FullName = "Shiv Nadar", DateOfBirth = new DateTime(1952, 7, 14), RemainingBalance = 190000 },
                new InsuranceMemberRegistry { MemberID = 47, PolicyNumber = "SH-3007", ProviderID = 3, PlanID = 5, FullName = "Adani Gautam", DateOfBirth = new DateTime(1962, 6, 24), RemainingBalance = 38000 },
                new InsuranceMemberRegistry { MemberID = 48, PolicyNumber = "SH-3008", ProviderID = 3, PlanID = 6, FullName = "Uday Kotak", DateOfBirth = new DateTime(1965, 3, 15), RemainingBalance = 200000 },
                new InsuranceMemberRegistry { MemberID = 49, PolicyNumber = "SH-3009", ProviderID = 3, PlanID = 5, FullName = "Savitri Jind", DateOfBirth = new DateTime(1958, 3, 20), RemainingBalance = 12000 },
                new InsuranceMemberRegistry { MemberID = 50, PolicyNumber = "SH-3010", ProviderID = 3, PlanID = 6, FullName = "Dilip Shangh", DateOfBirth = new DateTime(1959, 10, 1), RemainingBalance = 175000 },
                new InsuranceMemberRegistry { MemberID = 51, PolicyNumber = "SH-3011", ProviderID = 3, PlanID = 5, FullName = "Sunil Mittal", DateOfBirth = new DateTime(1963, 10, 23), RemainingBalance = 28000 },
                new InsuranceMemberRegistry { MemberID = 52, PolicyNumber = "SH-3012", ProviderID = 3, PlanID = 6, FullName = "Kumar Birla", DateOfBirth = new DateTime(1967, 6, 14), RemainingBalance = 160000 },
                new InsuranceMemberRegistry { MemberID = 53, PolicyNumber = "SH-3013", ProviderID = 3, PlanID = 5, FullName = "Cyrus Poonaw", DateOfBirth = new DateTime(1950, 5, 15), RemainingBalance = 10000 },
                new InsuranceMemberRegistry { MemberID = 54, PolicyNumber = "SH-3014", ProviderID = 3, PlanID = 6, FullName = "Radhakrish", DateOfBirth = new DateTime(1954, 8, 11), RemainingBalance = 145000 },
                new InsuranceMemberRegistry { MemberID = 55, PolicyNumber = "SH-3015", ProviderID = 3, PlanID = 5, FullName = "Pankaj Patel", DateOfBirth = new DateTime(1961, 9, 21), RemainingBalance = 33000 },
                new InsuranceMemberRegistry { MemberID = 56, PolicyNumber = "SH-3016", ProviderID = 3, PlanID = 6, FullName = "Nikhil Kamat", DateOfBirth = new DateTime(1987, 5, 31), RemainingBalance = 195000 },
                new InsuranceMemberRegistry { MemberID = 57, PolicyNumber = "SH-3017", ProviderID = 3, PlanID = 5, FullName = "Vijay Sharma", DateOfBirth = new DateTime(1973, 12, 12), RemainingBalance = 21000 },
                new InsuranceMemberRegistry { MemberID = 58, PolicyNumber = "SH-3018", ProviderID = 3, PlanID = 6, FullName = "Deepinder G", DateOfBirth = new DateTime(1984, 1, 26), RemainingBalance = 130000 },
                new InsuranceMemberRegistry { MemberID = 59, PolicyNumber = "SH-3019", ProviderID = 3, PlanID = 5, FullName = "Bhavish Agg", DateOfBirth = new DateTime(1986, 8, 28), RemainingBalance = 19000 },
                new InsuranceMemberRegistry { MemberID = 60, PolicyNumber = "SH-3020", ProviderID = 3, PlanID = 6, FullName = "Kunal Shah", DateOfBirth = new DateTime(1983, 5, 20), RemainingBalance = 120000 }
            );

            // Configure Prescription relationships
            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.HasOne(p => p.Appointment)
                      .WithMany(a => a.Prescriptions)
                      .HasForeignKey(p => p.AppointmentId);

                entity.HasOne(p => p.Medicine)
                      .WithMany()
                      .HasForeignKey(p => p.MedicineId);
            });
        }
    }
}
