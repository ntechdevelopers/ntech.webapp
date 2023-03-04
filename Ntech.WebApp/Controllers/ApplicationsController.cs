using Microsoft.AspNetCore.Mvc;

namespace Ntech.WebApp.Controllers
{
    public class ApplicationsController : Controller
    {
        public ApplicationsController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}
