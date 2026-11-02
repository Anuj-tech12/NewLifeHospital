using Microsoft.AspNetCore.Mvc;
using NewLifeHospital.Models;
using NewLifeHospital.Services;
using System.Diagnostics;

namespace NewLifeHospital.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmailService _emailService;
        private readonly ILogger<HomeController> _logger;


    public HomeController(
        EmailService emailService,
        ILogger<HomeController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _emailService.SendContactEmailAsync(model);

                TempData["ContactSuccess"] =
                    "Thank you! Your message has been sent successfully.";

                return RedirectToAction(nameof(Contact));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact form email.");


ModelState.AddModelError(
    string.Empty,
    $"Email error: {ex.GetBaseException().Message}");

                return View(model);


}

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }


}
