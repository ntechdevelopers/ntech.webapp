using Microsoft.AspNetCore.Mvc;

namespace Ntech.WebApp.Controllers
{
    public class AssetsController : Controller
    {
        public AssetsController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
