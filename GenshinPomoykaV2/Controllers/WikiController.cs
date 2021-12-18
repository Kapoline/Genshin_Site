using GenshinPomoykaV2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenshinPomoykaV2.Controllers
{
    [Authorize]
    public class WikiController : Controller
    {
        private readonly DataRepository _data;

        public WikiController(DataRepository data)
        {
            _data = data;
        }
        
        

        public IActionResult ShowCharacters()
        {
            return View();
        }

        public IActionResult ShowWeapons()
        {
            return View();
        }
    }
}