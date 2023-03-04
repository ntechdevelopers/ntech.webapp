using Ntech.WebApp.Data;
using Ntech.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ntech.WebApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var result = new DashboardVM()
            {
                NumberOfStaffs = 0,
                NumberOfCustomers = 0,
            };
            return View(result);
        }
    }
}