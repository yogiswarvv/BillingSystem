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
                entity.HasIndex(p => p.MobileNumber).HasDatabaseName("IX_Patient_MobileNumber");
                entity.Property(p => p.FullName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.MobileNumber).IsRequired().HasMaxLength(10);
                entity.Property(p => p.CreatedDate).HasDefaultValueSql("GETDATE()");
            });

            // 2. ServiceMaster
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
                new Patient { PatientId = 1, FullName = "John Doe", Age = 45, Gender = Gender.Male, MobileNumber = "9876543210", IsSenior = false, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) },
                new Patient { PatientId = 2, FullName = "Jane Smith", Age = 65, Gender = Gender.Female, MobileNumber = "8765432109", IsSenior = true, IsActive = true, CreatedDate = new DateTime(2024, 1, 1) }
            );

            modelBuilder.Entity<ServiceMaster>().HasData(
                new ServiceMaster { ServiceId = 1, ServiceName = "Lab Test", Cost = 600, IsActive = true, Department = "Pathology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 2, ServiceName = "X-Ray", Cost = 300, IsActive = true, Department = "Radiology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 3, ServiceName = "ECG", Cost = 1500, IsActive = true, Department = "Cardiology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 4, ServiceName = "MRI", Cost = 2500, IsActive = true, Department = "Radiology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 5, ServiceName = "Ultrasound", Cost = 1200, IsActive = true, Department = "Radiology", CreatedDate = new DateTime(2024, 1, 1) },
                new ServiceMaster { ServiceId = 6, ServiceName = "Blood Work", Cost = 400, IsActive = true, Department = "Pathology", CreatedDate = new DateTime(2024, 1, 1) }
            );
        }
    }
}
