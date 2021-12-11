using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenshinPomoykaV2.Controllers
{
    public class HomeController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}