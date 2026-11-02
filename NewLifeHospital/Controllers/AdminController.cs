using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLifeHospital.Data;
using NewLifeHospital.Services;

namespace NewLifeHospital.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;


    public AdminController(
        ApplicationDbContext context,
        EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // Admin Dashboard
        public async Task<IActionResult> Index(string? search, DateTime? date)
        {
            ViewBag.TotalAppointmentsCount =
                await _context.Appointments.CountAsync();

            ViewBag.TodayAppointmentsCount =
                await _context.Appointments.CountAsync(
                    a => a.AppointmentDate.Date == DateTime.Today);

            ViewBag.PendingAppointmentsCount =
                await _context.Appointments.CountAsync(
                    a => a.Status == "Pending");

            ViewBag.ApprovedAppointmentsCount =
                await _context.Appointments.CountAsync(
                    a => a.Status == "Approved");

            ViewBag.DoctorWiseAppointments =
                await _context.Appointments
                    .GroupBy(a => a.Doctor)
                    .Select(g => new
                    {
                        Doctor = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(g => g.Count)
                    .ToListAsync();

            var query = _context.Appointments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.PatientName.Contains(search) ||
                    a.Phone.Contains(search));
            }

            if (date.HasValue)
            {
                query = query.Where(a =>
                    a.AppointmentDate.Date == date.Value.Date);
            }

            var appointments = await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            ViewBag.SearchTerm = search;
            ViewBag.SelectedDate = date?.ToString("yyyy-MM-dd");

            return View(appointments);
        }

        // View Patient Details
        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // Approve or Reject Appointment + Send Email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (status != "Approved" && status != "Rejected")
            {
                return BadRequest();
            }

            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = status;

            // Save status in database
            await _context.SaveChangesAsync();

            // Send email to patient
            try
            {
                await _emailService.SendAppointmentStatusEmailAsync(appointment);

                TempData["SuccessMessage"] =
                    $"Appointment {status} successfully. Email sent to patient.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] =
                    $"Appointment {status}, but email could not be sent. Please check the patient's email address and SMTP settings.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Delete Appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }


}
