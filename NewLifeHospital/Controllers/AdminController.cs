using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewLifeHospital.Data;

namespace NewLifeHospital.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, DateTime? date)
        {
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

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }


}
