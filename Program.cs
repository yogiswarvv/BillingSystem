using BillingSystem.Data;
using BillingSystem.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BillingSystem.Data.HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped(typeof(BillingSystem.Repositories.IRepository<>), typeof(BillingSystem.Repositories.Repository<>));
builder.Services.AddScoped<BillingSystem.Repositories.IUnitOfWork, BillingSystem.Repositories.UnitOfWork>();

// Services
builder.Services.AddScoped<BillingSystem.Services.IBillingService, BillingSystem.Services.BillingService>();
builder.Services.AddScoped<BillingSystem.Services.IPatientService, BillingSystem.Services.PatientService>();
builder.Services.AddScoped<BillingSystem.Services.IPaymentService, BillingSystem.Services.PaymentService>();
builder.Services.AddScoped<BillingSystem.Services.IInsuranceService, BillingSystem.Services.InsuranceService>();
builder.Services.AddScoped<BillingSystem.Services.DoctorService>();
builder.Services.AddScoped<BillingSystem.Services.AppointmentService>();
builder.Services.AddScoped<BillingSystem.Services.LabOrderService>();

var app = builder.Build();

// Ensure Database is Created and Seeded
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<HospitalDbContext>();
        // Check if we can connect
        if (context.Database.CanConnect())
        {
            // If DB exists, EnsureCreated won't seed. We manually seed if empty.
            if (!context.Patients.Any())
            {
                context.Patients.Add(new Patient { FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1980, 1, 1), Gender = Gender.Male, MobileNumber = "9876543210", IsActive = true, CreatedDate = DateTime.Now });
                context.Patients.Add(new Patient { FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateTime(1960, 1, 1), Gender = Gender.Female, MobileNumber = "8765432109", IsActive = true, CreatedDate = DateTime.Now });
                context.SaveChanges();
            }
        }
        else
        {
            context.Database.EnsureCreated();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
