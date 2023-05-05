using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManager.Areas.Identity.Data;
using ProjectManager.Models;
using System.Diagnostics;

namespace ProjectManager.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Overview", "Projects");
            }
            else
            {
                return View();
            }

        }

        public IActionResult About()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}