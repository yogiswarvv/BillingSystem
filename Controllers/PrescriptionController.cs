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
            
            if (appointment.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Cannot add prescriptions to a cancelled appointment.";
                return RedirectToAction("Details", "Appointment", new { id = appointmentId });
            }

            var model = new BulkPrescriptionVM
            {
                AppointmentId = appointmentId,
                PatientName = appointment.Patient.FullName,
                Medicines = medicines.Select(m => new MedicineSelectionVM
                {
                    MedicineId = m.MedicineId,
                    Name = m.Name,
                    Dosage = m.DosageStrength ?? "N/A",
                    IsSelected = false,
                    Quantity = 1
                }).ToList()
            };

            return View("BulkCreate", model);
        }

        [HttpPost]
        public async Task<IActionResult> BulkCreate(BulkPrescriptionVM model)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(model.AppointmentId);
            if (appointment != null && appointment.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Cannot add prescriptions to a cancelled appointment.";
                return RedirectToAction("Details", "Appointment", new { id = model.AppointmentId });
            }

            var selectedMedications = model.Medicines.Where(m => m.IsSelected).ToList();

            if (selectedMedications.Any())
            {
                foreach (var item in selectedMedications)
                {
                    var prescription = new Prescription
                    {
                        AppointmentId = model.AppointmentId,
                        MedicineId = item.MedicineId,
                        SuggestedQuantity = item.Quantity,
                        ActualQuantity = item.Quantity,
                        Status = "Suggested",
                        IsPaid = false
                    };
                    _context.Prescriptions.Add(prescription);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Appointment", new { id = model.AppointmentId });
            }

            // Re-populate if fails or none selected
            var medicines = await _context.Medicines.Where(m => m.IsActive).ToListAsync();
            model.Medicines = medicines.Select(m => new MedicineSelectionVM
            {
                MedicineId = m.MedicineId,
                Name = m.Name,
                Dosage = m.DosageStrength ?? "N/A",
                IsSelected = false,
                Quantity = 1
            }).ToList();

            return View("BulkCreate", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PrescriptionCreateVM model)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(model.AppointmentId);
            if (appointment != null && appointment.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Cannot add prescriptions to a cancelled appointment.";
                return RedirectToAction("Details", "Appointment", new { id = model.AppointmentId });
            }

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
        [HttpPost]
        public async Task<IActionResult> UpdateActualQuantity(int id, int quantity)
        {
            if (quantity < 0) return BadRequest("Quantity cannot be negative.");

            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null) return NotFound();

            prescription.ActualQuantity = quantity;
            // Note: We don't change status to Purchased here, that happens when the bill is PAID.
            
            _context.Prescriptions.Update(prescription);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}
