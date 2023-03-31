using Microsoft.AspNetCore.Mvc;
using Pustok_BackEndProject.ViewModels.BasketViewModels;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Pustok_BackEndProject.Controllers
{
	public class BasketController : Controller
	{
		private readonly AppDbContext _context;

		public BasketController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			string cookie = HttpContext.Request.Cookies["basket"];
			List<BasketVM> basketVMs = null;

			if (!string.IsNullOrEmpty(cookie))
			{
				basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

				foreach (BasketVM basketVM in basketVMs)
				{
					Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == basketVM.Id);

					if (product != null)
					{
						basketVM.Title = product.Title;
						basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
						basketVM.Image = product.MainImage;
						basketVM.ExTax = product.Extax;
					}
				}
			}
			return View(basketVMs);
		}
		public async Task<IActionResult> RemoveBasket(int? Id)
		{
			if (Id == null) return BadRequest();
			string cookie = HttpContext.Request.Cookies["basket"];
			if (cookie == null) return BadRequest();
			List<BasketVM> basketVMs = null;
			if (!string.IsNullOrWhiteSpace(cookie))
			{
				basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
				if (basketVMs.Exists(p => p.Id == Id))
				{
					basketVMs.RemoveAll(p => p.Id == Id);
				}
				cookie = JsonConvert.SerializeObject(basketVMs);
				HttpContext.Response.Cookies.Append("basket", cookie);

				foreach (BasketVM basketVM in basketVMs)
				{
					Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == basketVM.Id);
					if (product != null)
					{
						basketVM.Title = product.Title;
						basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
						basketVM.Image = product.MainImage;
						basketVM.ExTax = product.Extax;
					}

				}


			}

			return PartialView("_BasketCartPartial", basketVMs);
		}
		public async Task<IActionResult> AddBasket(int? Id)
		{
			if (Id == null) return BadRequest();

			if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == Id)) return NotFound();

			string cookie = HttpContext.Request.Cookies["basket"];

			List<BasketVM> basketVMs = null;


			if (string.IsNullOrWhiteSpace(cookie))
			{
				basketVMs = new List<BasketVM>
				{
					new BasketVM {Id = (int)Id, Count= 1}
				};
			}
			else
			{
				basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
				if (basketVMs.Exists(p => p.Id == Id))
				{
					basketVMs.Find(b => b.Id == Id).Count += 1;
				}
				else
				{
					basketVMs.Add(new BasketVM { Id = (int)Id, Count = 1 });
				};
			}

			cookie = JsonConvert.SerializeObject(basketVMs);
			HttpContext.Response.Cookies.Append("basket", cookie);
				
			foreach (BasketVM basketVM in basketVMs)
			{
				Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == basketVM.Id);
				if (product != null)
				{
					basketVM.Title = product.Title;
					basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
					basketVM.Image = product.MainImage;
					basketVM.ExTax = product.Extax;
				}

			}

			return PartialView("_BasketCartPartial", basketVMs);


		}
		public async Task<IActionResult> GetBasket()
		{
			string basket = HttpContext.Request.Cookies["basket"];

			List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

			return Json(basketVMs);
		}

		public async Task<IActionResult> MainBasket()
		{
			string cookie = HttpContext.Request.Cookies["basket"];
			List<BasketVM> basketVMs = null;

			if (!string.IsNullOrEmpty(cookie))
			{
				basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

				foreach (BasketVM basketVM in basketVMs)
				{
					Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == basketVM.Id);

					if (product != null)
					{
						basketVM.Title = product.Title;
						basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
						basketVM.Image = product.MainImage;
						basketVM.ExTax = product.Extax;
					}
				}
			}

			return PartialView("_BasketIndexPartial", basketVMs);
		}
		public async Task<IActionResult> RefreshBasketMain()
		{
			string cookie = HttpContext.Request.Cookies["basket"];

			List<BasketVM> basketVMs = null;


			if (string.IsNullOrWhiteSpace(cookie))
			{
				return BadRequest();
			}

			basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);

			foreach (BasketVM basketVM in basketVMs)
			{
				Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == basketVM.Id);
				if (product != null)
				{
					basketVM.Title = product.Title;
					basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
					basketVM.Image = product.MainImage;
					basketVM.ExTax = product.Extax;
				}

			}

			return PartialView("_BasketIndexPartial", basketVMs);
		}
		public async Task<IActionResult> DecreaseBasket(int? Id)
		{
			if (Id == null) return BadRequest();

			if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == Id)) return NotFound();

			string cookie = HttpContext.Request.Cookies["basket"];

			List<BasketVM> basketVMs = null;


			if (string.IsNullOrWhiteSpace(cookie))
			{
				return BadRequest();
			}
			else
			{
				basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
				if (basketVMs.Exists(p => p.Id == Id) && basketVMs.Find(b => b.Id == Id).Count > 1)
				{
					basketVMs.Find(b => b.Id == Id).Count -= 1;
				}
				else if (basketVMs.Exists(p => p.Id == Id))
				{
					basketVMs.RemoveAll(p => p.Id == Id);
				}

			}
			cookie = JsonConvert.SerializeObject(basketVMs);
			HttpContext.Response.Cookies.Append("basket", cookie);

			foreach (BasketVM basketVM in basketVMs)
			{
				Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == basketVM.Id);
				if (product != null)
				{
					basketVM.Title = product.Title;
					basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
					basketVM.Image = product.MainImage;
					basketVM.ExTax = product.Extax;
				}

			}

			return PartialView("_BasketCartPartial", basketVMs);
		}
	}
}
