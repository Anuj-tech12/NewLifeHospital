using Microsoft.AspNetCore.Mvc;

namespace NewLifeHospital.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
