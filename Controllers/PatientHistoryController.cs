using BillingSystem.Services;
using BillingSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.Controllers
{
    public class PatientHistoryController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly AppointmentService _appointmentService;
        private readonly LabOrderService _labOrderService;
        private readonly IBillingService _billingService;

        public PatientHistoryController(
            IPatientService patientService, 
            AppointmentService appointmentService, 
            LabOrderService labOrderService,
            IBillingService billingService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
            _labOrderService = labOrderService;
            _billingService = billingService;
        }

        public async Task<IActionResult> Index(int patientId)
        {
            var patient = await _patientService.GetPatientDetailsAsync(patientId);
            if (patient == null) return NotFound();

            var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            var labOrders = await _labOrderService.GetAllLabOrdersAsync();
            var bills = await _billingService.GetBillsByPatientIdAsync(patientId);

            var model = new PatientHistoryViewModel
            {
                Patient = patient,
                Appointments = appointments.ToList(),
                LabOrders = labOrders.Where(l => l.Appointment.PatientId == patientId).ToList(),
                Bills = bills.ToList()
            };

            return View(model);
        }
    }

    public class PatientHistoryViewModel
    {
        public Models.Patient Patient { get; set; } = null!;
        public List<Models.Appointment> Appointments { get; set; } = new();
        public List<Models.LabOrder> LabOrders { get; set; } = new();
        public List<Models.Bill> Bills { get; set; } = new();
    }
}
