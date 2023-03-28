using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Models;


namespace Pustok_BackEndProject.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BrandController : Controller
    {
        public IActionResult Index() 
        { 
            return View();
        }
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }
        //public async Task<IActionResult> Index(int? Id)
        //{

        //    IEnumerable<Brand> brands = await _context.Brands.Include(b => b.Products).Where(b => b.IsDeleted == false).ToListAsync();
        //    //if (Id == null) return BadRequest();
        //    //Brand brand = await _context.Brands
        //    //    .Include(b => b.Products.Where(p => p.IsDeleted == false))
        //    //    .FirstOrDefaultAsync(b => b.Id == Id && b.IsDeleted == false);
        //    //if (brand == null) return NotFound();
        //    return View(brands);
        //}
    }
}
