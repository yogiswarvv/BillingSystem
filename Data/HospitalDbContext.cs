using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
        {
        }

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
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Schema
            modelBuilder.HasDefaultSchema("Healthcare");

            // --- FLUENT API CONFIGURATION ---

            // 1. Patient
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patients", "Healthcare");
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
                entity.ToTable("Appointment", "Healthcare");
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
                entity.ToTable("Doctor", "Healthcare");
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
                entity.ToTable("LabOrder", "Healthcare");
                entity.HasKey(l => l.LabOrderId);
                entity.Property(l => l.Status).HasDefaultValue("Pending");
                entity.Property(l => l.OrderDate).HasDefaultValueSql("GETDATE()");
            });

            // 5. AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLog", "Healthcare");
                entity.Property(a => a.ChangedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(a => a.ChangedBy).HasDefaultValue("SYSTEM");
            });

            // 6. ServiceMaster
            modelBuilder.Entity<ServiceMaster>(entity =>
            {
                entity.HasIndex(s => s.ServiceName).IsUnique().HasDatabaseName("IX_Service_Name");
                entity.Property(s => s.Cost).HasColumnType("decimal(18,2)");
                entity.Property(s => s.CreatedDate).HasDefaultValueSql("GETDATE()");
            });

            // 3. Bill & BillItem & Payment (Money Precision)
            modelBuilder.Entity<Bill>(entity => 
            {
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
                entity.HasIndex(p => p.TransactionRef).HasDatabaseName("IX_Payment_TransactionRef");
                entity.Property(p => p.PaidAmount).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Admission>(entity =>
            {
                entity.Property(a => a.FeePerDay).HasColumnType("decimal(18,2)").HasDefaultValue(2000m);
            });

            // --- RELATIONSHIPS (ON DELETE RESTRICT) ---
            
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Patient)
                .WithMany(p => p.Bills)
                .HasForeignKey(b => b.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Admission>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Admissions)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Insurance>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Insurances)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

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
        }
    }
}
