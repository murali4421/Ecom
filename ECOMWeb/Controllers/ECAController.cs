using EcomCore;
using Microsoft.AspNetCore.Mvc;

namespace ECOMWeb.Controllers
{
    public class ECAController : Controller
    {
        HttpContextAccessor context = new();
        [HttpGet]
        public IActionResult Login()
        {            
            context.HttpContext.Session.SetString("Dashboard", "NA");
            return View();
        }
        [HttpPost]
        public IActionResult Login(Voters voters)
        {
            context.HttpContext.Session.SetString("Dashboard", "AD");
            return RedirectToAction("Index","Home");
        }
    }
}
