using Microsoft.AspNetCore.Mvc;

namespace Ntech.WebApp.Controllers
{
    public class DevicesController : Controller
    {
        public DevicesController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
