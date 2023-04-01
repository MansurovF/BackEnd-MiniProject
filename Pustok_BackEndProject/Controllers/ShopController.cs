using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels.ShopViewModels;

namespace Pustok_BackEndProject.Controllers
{
    public class ShopController : Controller
    {
		private readonly AppDbContext _context;

		public ShopController(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
        {
			List<Product> products= await _context.Products.Where(s=>s.IsDeleted == false).ToListAsync();
			ShopVM shopVM = new ShopVM
			{
				Products = products
			}; 


            return View(shopVM);
        }

		public async Task<IActionResult> Range(double? min,double? max)
		{
			List<Product> products = await _context.Products.Where(s => s.IsDeleted == false && s.Price >= min && s.Price <= max).ToListAsync();
			ShopVM shopVM = new ShopVM
			{
				Products = products
			};


			return PartialView("_ShopViewPartial",products);
		}
    }
}
