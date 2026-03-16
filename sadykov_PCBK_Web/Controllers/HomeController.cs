using Microsoft.AspNetCore.Mvc;

namespace sadykov_PCBK_Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "Partners");
    }
}
