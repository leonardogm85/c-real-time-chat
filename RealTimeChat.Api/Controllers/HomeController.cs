using Microsoft.AspNetCore.Mvc;

namespace RealTimeChat.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Chat()
        {
            return View();
        }
    }
}
