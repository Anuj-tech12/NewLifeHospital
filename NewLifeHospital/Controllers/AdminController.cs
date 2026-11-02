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

        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

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
