using Microsoft.AspNetCore.Mvc;

namespace Pustok_BackEndProject.Controllers
{
	public class BlogController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
