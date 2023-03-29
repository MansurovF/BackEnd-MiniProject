using Microsoft.AspNetCore.Mvc;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.ViewModels;
using Pustok_BackEndProject.Models;
using Microsoft.EntityFrameworkCore;

namespace Pustok_BackEndProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task <IActionResult>Index()
        {
            return View(await _context.Categories
                .Include(c=>c.Products.Where(p=>p.IsDeleted == false))
                .Where(c=>c.IsDeleted == false && c.IsMain)
                .ToListAsync());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file)
        {
            //ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();

            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}
            //if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower()))
            //{
            //    ModelState.AddModelError("Name", $"{category.Name} add categoryartiq movcuddur!");
            //    return View(category);
            //}

            return RedirectToAction(nameof(Index));
        }

    }
}
