using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NewLifeHospital.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email,
            string password,
            string? returnUrl = null)
        {
            var result = await _signInManager.PasswordSignInAsync(
    email,
    password,
    isPersistent: false,
    lockoutOnFailure: true
);

            if (result.IsLockedOut)
            {
                ViewBag.Error = "Your account is locked for 15 minutes. Please try again later.";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }


            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Admin");
            }

            ViewBag.Error = "Invalid email or password.";
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}