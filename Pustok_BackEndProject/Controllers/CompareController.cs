using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels.CompareViewModel;
using Pustok_BackEndProject.ViewModels.WishListViewModels;
using Newtonsoft.Json;

namespace MiniBackEnd.Controllers
{
	public class CompareController : Controller
	{
		private readonly AppDbContext _context;

		public CompareController(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{

			string cookie = HttpContext.Request.Cookies["compare"];
			List<CompareVM> compareVMs = null;

			if (!string.IsNullOrEmpty(cookie))
			{
				compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);

				foreach (CompareVM compareVM in compareVMs)
				{
					Product product = await _context.Products.Include(p => p.Reviews).Include(p => p.Category).FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == compareVM.Id);

					if (product != null)
					{
						compareVM.Title = product.Title;
						compareVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
						compareVM.Image = product.MainImage;
						compareVM.Description = product.SecondDescription;
						compareVM.Review = (product.Reviews?.Count > 0 && product.Reviews?.Count != null ? (int?)product.Reviews?.Average(r => r.Start) : null);
						compareVM.InStock = product.Count > 0 ? true : false;
						compareVM.CategoryName = product.Category?.Name;
					}
				}
			}


			return View(compareVMs);
		}
		public async Task<IActionResult> RemoveCompare(int? id)
		{
			if (id == null) return BadRequest();
			if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();

			if (id == null) return BadRequest();
			string cookie = HttpContext.Request.Cookies["compare"];
			if (cookie == null) return BadRequest();
			List<CompareVM> compareVMs = null;
			if (!string.IsNullOrWhiteSpace(cookie))
			{
				compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);
				if (compareVMs.Exists(p => p.Id == id))
				{
					compareVMs.RemoveAll(p => p.Id == id);
				}
				cookie = JsonConvert.SerializeObject(compareVMs);
				HttpContext.Response.Cookies.Append("compare", cookie);

				foreach (CompareVM compareVM in compareVMs)
				{
					Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == compareVM.Id);
					if (product != null)
					{
						compareVM.Id = product.Id;
						compareVM.Title = product.Title;
						compareVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
						compareVM.Image = product.MainImage;
						compareVM.Description = product.SecondDescription;
						compareVM.Review = (product.Reviews?.Count > 0 && product.Reviews?.Count != null ? (int?)product.Reviews?.Average(r => r.Start) : null);

						compareVM.InStock = product.Count > 0 ? true : false;
						compareVM.CategoryName = product.Category?.Name;

					}

				}


			}
			return PartialView("_CompareIndexPartial", compareVMs);
		}
		public async Task<IActionResult> CheckCompare(int? id)
		{
			if (id == null) return BadRequest();

			if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();


			string cookie = HttpContext.Request.Cookies["compare"];

			List<CompareVM> compareVMs = null;

			if (string.IsNullOrWhiteSpace(cookie))
			{
				compareVMs = new List<CompareVM>
				{
					new CompareVM {Id = (int)id}
				};


			}
			else
			{
				compareVMs = JsonConvert.DeserializeObject<List<CompareVM>>(cookie);
				if (compareVMs.Exists(p => p.Id == id))
				{
					compareVMs.RemoveAll(b => b.Id == id);
				}
				else
				{
					if (compareVMs.Count == 4)
					{
						compareVMs.RemoveAt(0);
					}
					compareVMs.Add(new CompareVM { Id = (int)id });
				};

			}

			cookie = JsonConvert.SerializeObject(compareVMs);
			HttpContext.Response.Cookies.Append("compare", cookie);

			foreach (CompareVM compareVM in compareVMs)
			{
				Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == compareVM.Id);
				if (product != null)
				{
					compareVM.Title = product.Title;
					compareVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
					compareVM.Image = product.MainImage;
					compareVM.Description = product.SecondDescription;
					compareVM.Review = (product.Reviews?.Count > 0 && product.Reviews?.Count != null ? (int?)product.Reviews?.Average(r => r.Start) : null);

					compareVM.InStock = product.Count > 0 ? true : false;
					compareVM.CategoryName = product.Category?.Name;

				}

			}

			return PartialView("_CompareCartPartial");
			//return View(compareVMs);
		}
	}
}
