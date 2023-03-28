using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Interfaces;
using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels.BasketViewModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using NuGet.ContentModel;

namespace Pustok_BackEndProject.Services
{
    public class LayoutService:ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public LayoutService(AppDbContext context, IHttpContextAccessor httpcontextAccessor)
        {
            _context = context;
            _httpcontextAccessor = httpcontextAccessor;
        }
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _context.Categories
                .Include(c => c.Children.Where(c => c.IsDeleted == false))
                .Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();
        }
        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }
        public async Task<List<BasketVM>> GetBasket()
        {
            List<Basket> baskets = null;

            string cookie = _httpcontextAccessor.HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                List<BasketVM> basketVMs = null;

                if (baskets != null && baskets.Count > 0)
                {
                    basketVMs = new List<BasketVM>();
                    foreach (Basket basket in baskets)
                    {   
                            Product product = basket.Product;
                            if (product != null)
                            {
                                BasketVM basketVM = new BasketVM();

                                basketVM.Id = product.Id;
                                basketVM.Count = basket.Count;
                                basketVM.Title = product.Title;
                                basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                                basketVM.Image = product.MainImage;
                                basketVM.ExTax = product.Extax;

                                basketVMs.Add(basketVM);
                            }
                    }
                }
                else
                {
                    basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(cookie);
                    foreach (BasketVM basketVM1 in basketVMs)
                    {
                        Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM1.Id);
                        if (product != null)
                        {
                            basketVM1.Title = product.Title;
                            basketVM1.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                            basketVM1.Image = product.MainImage;
                            basketVM1.ExTax = product.Extax;
                        }

                    }
                }
                return basketVMs;
            }
            return new List<BasketVM>();
        }
    }
}
