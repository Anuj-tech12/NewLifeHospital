using Microsoft.AspNetCore.Mvc;
using NewLifeHospital.Data;
using NewLifeHospital.Models;

namespace NewLifeHospital.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(
            string PatientName,
            string Phone,
            string? Email,
            string Doctor,
            DateTime AppointmentDate,
            string AppointmentTime,
            string? Message)
        {
            if (string.IsNullOrWhiteSpace(PatientName) ||
                string.IsNullOrWhiteSpace(Phone) ||
                string.IsNullOrWhiteSpace(Doctor) ||
                string.IsNullOrWhiteSpace(AppointmentTime))
            {
                ViewBag.ErrorMessage = "Please fill in all required fields.";
                return View();
            }

            if (AppointmentDate.Date < DateTime.Today)
            {
                ViewBag.ErrorMessage = "Please select today or a future date.";
                return View();
            }

            var appointment = new Appointment
            {
                PatientName = PatientName,
                Phone = Phone,
                Email = Email,
                Doctor = Doctor,
                AppointmentDate = AppointmentDate,
                AppointmentTime = AppointmentTime,
                Message = Message
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            ViewBag.PatientName = PatientName;
            ViewBag.Doctor = Doctor;
            ViewBag.AppointmentDate = AppointmentDate.ToString("dd MMMM yyyy");
            ViewBag.AppointmentTime = AppointmentTime;

            return View("Confirmation");
        }
    }
}