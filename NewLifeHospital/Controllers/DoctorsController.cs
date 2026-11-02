using Microsoft.AspNetCore.Mvc;

namespace NewLifeHospital.Controllers
{
    public class DoctorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
