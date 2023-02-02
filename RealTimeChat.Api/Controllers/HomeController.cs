using Microsoft.AspNetCore.Mvc;

namespace RealTimeChat.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult LogIn()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Talk()
        {
            return View();
        }
    }
}
