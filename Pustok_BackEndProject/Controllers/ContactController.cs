using Microsoft.AspNetCore.Mvc;

namespace Pustok_BackEndProject.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
