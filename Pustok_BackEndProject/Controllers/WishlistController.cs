using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels.BasketViewModels;
using Pustok_BackEndProject.ViewModels.WishListViewModels;
using Newtonsoft.Json;

namespace MiniBackEnd.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            string cookie = HttpContext.Request.Cookies["wish"];
            List<WishlistVM> wishlistVMs = null;

            if (!string.IsNullOrEmpty(cookie))
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);

                foreach (WishlistVM wishlistVM in wishlistVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == wishlistVM.Id);

                    if (product != null)
                    {
                        wishlistVM.Title = product.Title;
                        wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        wishlistVM.Image = product.MainImage;
                        wishlistVM.ExTax = product.Extax;
                    }
                }
            }


            return View(wishlistVMs);
        }
        public async Task<IActionResult> RemoveWish(int? id)
        {
            if (id == null) return BadRequest();
            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();

            if (id == null) return BadRequest();
            string cookie = HttpContext.Request.Cookies["wish"];
            if (cookie == null) return BadRequest();
            List<WishlistVM> wishlistVMs = null;
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);
                if (wishlistVMs.Exists(p => p.Id == id))
                {
                    wishlistVMs.RemoveAll(p => p.Id == id);
                }
                cookie = JsonConvert.SerializeObject(wishlistVMs);
                HttpContext.Response.Cookies.Append("wish", cookie);

                foreach (WishlistVM wishlistVM in wishlistVMs)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == wishlistVM.Id);
                    if (product != null)
                    {
                        wishlistVM.Title = product.Title;
                        wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        wishlistVM.Image = product.MainImage;
                        wishlistVM.ExTax = product.Extax;
                    }

                }


            }
            return PartialView("_WishListIndexPartial",wishlistVMs);
        }
        public async Task<IActionResult> CheckWish(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id)) return NotFound();


            string cookie = HttpContext.Request.Cookies["wish"];

            List<WishlistVM> wishlistVMs = null;

            if (string.IsNullOrWhiteSpace(cookie))
            {
                wishlistVMs = new List<WishlistVM>
                {
                    new WishlistVM {Id = (int)id}
                };


            }
            else
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(cookie);
                if (wishlistVMs.Exists(p => p.Id == id))
                {
                    wishlistVMs.RemoveAll(b => b.Id == id);
                }
                else
                {
                    wishlistVMs.Add(new WishlistVM { Id = (int)id});
                };

            }

            cookie = JsonConvert.SerializeObject(wishlistVMs);
            HttpContext.Response.Cookies.Append("wish", cookie);

            foreach (WishlistVM wishlistVM in wishlistVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == wishlistVM.Id);
                if (product != null)
                {
                    wishlistVM.Title = product.Title;
                    wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                    wishlistVM.Image = product.MainImage;
                    wishlistVM.ExTax = product.Extax;
                }

            }

            return PartialView("_WishCartPartial");
        }
    }
}
