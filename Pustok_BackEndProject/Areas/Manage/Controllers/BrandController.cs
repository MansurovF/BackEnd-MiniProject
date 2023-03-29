using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels;
using System.Data;


namespace Pustok_BackEndProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Brand> query = _context.Brands
                 .Include(b => b.Products)
                 .Where(b => b.IsDeleted == false);
            return View(PageNatedList<Brand>.Create(query, pageIndex, 3, 3));
        }

        public async Task<IActionResult> Detail(int? Id)
        {
            if (Id == null) return BadRequest();
            Brand brand = await _context.Brands
                .Include(b => b.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.Id == Id && b.IsDeleted == false);
            if (brand == null) return NotFound();
            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View(brand);
            }
            if (await _context.Brands.AnyAsync(b=>b.IsDeleted ==false && b.Name.ToLower() == brand.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"{brand.Name} adinda brand movcuddur");
                return View(brand);
            }
            brand.Name = brand.Name.Trim();
            brand.CreatedAt = DateTime.UtcNow.AddHours(4);
            brand.CreatedBy = "System";


            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? Id)
        {
            if (Id == null) return BadRequest();
            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == Id && b.IsDeleted == false);
            if (brand == null) return NotFound();
            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? Id, Brand brand)
        {
            if (!ModelState.IsValid) return View(brand);
            if (Id == null) return BadRequest();
            if (Id != brand.Id) return BadRequest();
            Brand dbBrand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == Id && b.IsDeleted == false);
            if (dbBrand == null) return NotFound();
            if (await _context.Brands.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower() == brand.Name.Trim().ToLower() && b.Id != Id))
            {
                ModelState.AddModelError("Name", $"Bu Adda {brand.Name} Brand Artiq Movcuddur");
                return View(brand);
            }

            dbBrand.Name = brand.Name.Trim();
            dbBrand.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbBrand.UpdatedBy = "System";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null) return BadRequest();
            Brand brand = await _context.Brands
                .Include(b => b.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.Id == Id && b.IsDeleted == false);
            if (brand == null) return NotFound();

            brand.IsDeleted = true;
            brand.DeletedBy = "System";
            brand.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            IEnumerable<Brand> brands = await _context.Brands.Include(b => b.Products).Where(b => b.IsDeleted == false).ToListAsync();

            return PartialView("_BrandIndexPartial", brand);


            //IQueryable<Brand> query = _context.Brands
            //    .Include(b => b.Products)
            //    .Where(b => b.IsDeleted == false);
            //int pageIndex;

            //return RedirectToAction(nameof(Index));
            //return PartialView("_BrandIndexPartial", PageNatedList<Brand>.Create(query, pageIndex = 1, 3, 3));
        }
    }
}
