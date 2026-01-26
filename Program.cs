using BillingSystem.Data;
using BillingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BillingSystem.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BillingSystem.Data.HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Comprehensive Identity Configuration (Using Slim Stores)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddUserStore<SlimUserStore>()
.AddRoleStore<SlimRoleStore>()
.AddDefaultTokenProviders();

// Role-based Cookie Configuration
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

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
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure Identity Tables / Data
        await context.Database.MigrateAsync();

        // 1. Seed Roles
        string[] roles = { "Admin", "Billing", "Lab" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Default Users
        var users = new[] 
        {
            new { Email = "admin@hospital.com", Role = "Admin", Pwd = "Admin@123" },
            new { Email = "billing@hospital.com", Role = "Billing", Pwd = "Billing@123" },
            new { Email = "lab@hospital.com", Role = "Lab", Pwd = "Lab@123" }
        };

        foreach (var u in users)
        {
            if (await userManager.FindByEmailAsync(u.Email) == null)
            {
                var user = new IdentityUser { UserName = u.Email, Email = u.Email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, u.Pwd);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, u.Role);
                }
            }
        }

        // 3. Seed Patient Data if empty
        if (!context.Patients.Any())
        {
            context.Patients.Add(new Patient { FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1980, 1, 1), Gender = Gender.Male, MobileNumber = "9876543210", IsActive = true, CreatedDate = DateTime.Now });
            context.Patients.Add(new Patient { FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateTime(1960, 1, 1), Gender = Gender.Female, MobileNumber = "8765432109", IsActive = true, CreatedDate = DateTime.Now });
            context.SaveChanges();
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
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
