using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Models;

namespace Pustok_BackEndProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Modal(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products.Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null)
            {
                return NotFound();
            }


            return PartialView("_ModalPartial", product);
            //return Ok(product);
        }
        public async Task<IActionResult> Search (string search,int? categoryId)
        {
            IEnumerable<Product> products = await _context.Products
                .Where(p =>
                p.IsDeleted == false &&
                (categoryId != null && categoryId > 0 &&
                _context.Categories.Any(c => c.IsDeleted == false && c.Id == categoryId) ? p.CategoryId == categoryId : true) &&
                ((p.Title.ToLower().Contains(search.Trim().ToLower()) ||
                    p.Brand.Name.ToLower().Contains(search.Trim().ToLower()) ||
                    p.Category.Name.ToLower().Contains(search.Trim().ToLower())))
                ).OrderByDescending(p => p.Id).Take(5).ToListAsync();

            return PartialView("_SearchPartial", products);
        }
    }
}
