using BillingSystem.Data;
using BillingSystem.Models;
using BillingSystem.Services;
using BillingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BillingSystem.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly HospitalDbContext _context;
        private readonly AppointmentService _appointmentService;

        public PrescriptionController(HospitalDbContext context, AppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int appointmentId)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null) return NotFound();

            var medicines = await _context.Medicines.Where(m => m.IsActive).ToListAsync();
            var model = new BulkPrescriptionVM
            {
                AppointmentId = appointmentId,
                PatientName = appointment.Patient.FullName,
                AvailableMedicines = medicines.Select(m => new SelectListItem 
                { 
                    Value = m.MedicineId.ToString(), 
                    Text = $"[{m.DosageStrength}] {m.Name}" 
                }).ToList()
            };

            return View("BulkCreate", model);
        }

        [HttpPost]
        public async Task<IActionResult> BulkCreate(BulkPrescriptionVM model)
        {
            if (model.SelectedMedicineIds != null && model.SelectedMedicineIds.Any())
            {
                foreach (var medId in model.SelectedMedicineIds)
                {
                    var prescription = new Prescription
                    {
                        AppointmentId = model.AppointmentId,
                        MedicineId = medId,
                        SuggestedQuantity = 1, // Defaulting to 1 for bulk, can be updated in Dispense
                        ActualQuantity = 1,
                        Status = "Suggested",
                        IsPaid = false
                    };
                    _context.Prescriptions.Add(prescription);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Appointment", new { id = model.AppointmentId });
            }

            // Re-populate if fails
            var medicines = await _context.Medicines.Where(m => m.IsActive).ToListAsync();
            model.AvailableMedicines = medicines.Select(m => new SelectListItem 
            { 
                Value = m.MedicineId.ToString(), 
                Text = $"[{m.DosageStrength}] {m.Name}" 
            }).ToList();

            return View("BulkCreate", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PrescriptionCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var prescription = new Prescription
                {
                    AppointmentId = model.AppointmentId,
                    MedicineId = model.MedicineId,
                    SuggestedQuantity = model.SuggestedQuantity,
                    ActualQuantity = model.SuggestedQuantity, // Default to suggested
                    Status = "Suggested",
                    IsPaid = false
                };
                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Appointment", new { id = model.AppointmentId });
            }

            var medicines = await _context.Medicines.Where(m => m.IsActive).ToListAsync();
            model.AvailableMedicines = medicines.Select(m => new SelectListItem 
            { 
                Value = m.MedicineId.ToString(), 
                Text = m.Name 
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Dispense(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Medicine)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);
            
            if (prescription == null) return NotFound();

            var model = new PrescriptionUpdateVM
            {
                PrescriptionId = prescription.PrescriptionId,
                MedicineName = prescription.Medicine.Name,
                Dosage = prescription.Medicine.DosageStrength ?? "",
                SuggestedQuantity = prescription.SuggestedQuantity,
                ActualQuantity = prescription.ActualQuantity
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Dispense(PrescriptionUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                var prescription = await _context.Prescriptions.FindAsync(model.PrescriptionId);
                if (prescription == null) return NotFound();

                prescription.ActualQuantity = model.ActualQuantity;
                prescription.Status = model.ActualQuantity > 0 ? "Purchased" : "Cancelled";
                
                _context.Prescriptions.Update(prescription);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Appointment", new { id = prescription.AppointmentId });
            }
            return View(model);
        }
    }
}
