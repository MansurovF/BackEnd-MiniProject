using Microsoft.AspNetCore.Mvc;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.ViewModels;
using Pustok_BackEndProject.Models;
using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.Extensions;
using Pustok_BackEndProject.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pustok_BackEndProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task <IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Category> query = _context.Categories
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .Where(c => c.IsDeleted == false && c.IsMain)
                .OrderByDescending(c => c.Id);

            return View(PageNatedList<Category>.Create(query, pageIndex, 3, 8));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int ?id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(ch => ch.IsDeleted == false))
                .ThenInclude(ch => ch.Products.Where(p => p.IsDeleted == false))
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View();
            }
            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"{category.Name} add categoryartiq movcuddur!");
                return View(category);
            }
            if (category.IsMain)
            {
                if (category.File?.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "Uygun Type Deyil, Yalniz JPEG/JPG type ola biler!");
                    return View();
                }
                if ((category.File?.Length / 1024) > 300)
                {
                    ModelState.AddModelError("File", "File-in olcusu 300Kb-i kece bilmez");
                }
                //category.im = await category.File.CreateFileAsync(_env, "assets", "image");
                category.ParentId = null;
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir!");
                    return View(category);
                }
                if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == category.ParentId && c.IsMain))
                {
                    ModelState.AddModelError("ParentId", "Parent duzgun secilmelidir!");
                    return View(category);
                }
            }
            category.Name = category.Name.Trim();
            category.CreatedAt = DateTime.UtcNow.AddHours(4);
            category.CreatedBy = "System";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult>Update(int? Id)
        {

            if (Id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == Id && c.IsDeleted == false);

            if (category == null) return NotFound();

            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();


            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            ViewBag.MainCategories = await _context.Categories.Where(c => c.IsDeleted == false && c.IsMain).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }
            if (id == null) return BadRequest();
            if (id != category.Id) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) return NotFound();

            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower() == category.Name.Trim().ToLower() && c.Id != category.Id))
            {
                ModelState.AddModelError("Name", $"{category.Name} add categoryartiq movcuddur!");
                return View(category);
            }

            if (dbCategory.IsMain != category.IsMain)
            {
                ModelState.AddModelError("isMain", "Category veziyyeti deyise bilmez");
                return View(category);
            }

            if (dbCategory.IsMain && category.File != null)
            {
                if (category.File.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("File", "Uygun Type Deyil, Yalniz JPEG/JPG type ola biler!");
                    return View();
                }
                if (category.File.CheckFileLenght(300))
                {
                    ModelState.AddModelError("File", "File-in olcusu 300Kb-i kece bilmez");
                }
            }
            else
            {
                if (category.ParentId != dbCategory.ParentId)
                {
                    if (category.ParentId == null)
                    {
                        ModelState.AddModelError("ParentId", "Parent mutleq secilmelidir!");
                        return View(category);
                    }
                    if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == category.ParentId && c.IsMain))
                    {
                        ModelState.AddModelError("ParentId", "Parent duzgun secilmelidir!");
                        return View(category);
                    }

                    dbCategory.ParentId = category.ParentId;
                }
            }

            dbCategory.Name = category.Name.Trim();
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbCategory.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Children.Where(ch => ch.IsDeleted == false))
                .ThenInclude(ch => ch.Products.Where(p => p.IsDeleted == false))
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);
            if (category == null) return NotFound();

            if (category.Children != null && category.Children.Count() > 0)
            {
                foreach (Category child in category.Children)
                {
                    child.IsDeleted = true;
                    child.DeletedBy = "System";
                    child.DeletedAt = DateTime.UtcNow.AddHours(4);

                    if (child.Products != null && child.Products.Count() > 0)
                    {
                        foreach (Product product in child.Products)
                        {
                            product.CategoryId = null;
                        }
                    }
                }
            }

            if (category.Products != null && category.Products.Count() > 0)
            {
                foreach (Product product in category.Products)
                {
                    product.CategoryId = null;
                }
            }

            category.IsDeleted = true;
            category.DeletedBy = "System";
            category.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IQueryable<Category> query = _context.Categories
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .Where(c => c.IsDeleted == false && c.IsMain)
                .OrderByDescending(c => c.Id);

            int pageIndex;

            return PartialView("_CategoryIndexPartial", PageNatedList<Category>.Create(query, pageIndex = 1, 3, 8));
        }
    }
}
