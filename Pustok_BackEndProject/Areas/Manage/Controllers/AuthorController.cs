using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels;
using System.Data;
using Microsoft.EntityFrameworkCore;


namespace MiniBackEnd.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AuthorController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorController(AppDbContext context)
        {
            _context = context;
        }

        //[Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Author> query = _context.Authors
                .Include(b => b.Products)
                .Where(b => b.IsDeleted == false);
            return View(PageNatedList<Author>.Create(query, pageIndex, 3, 3));
        }

        //[Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Detail(int? Id)
        {
            if (Id == null) return BadRequest();
            Author author = await _context.Authors
                .Include(b => b.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.Id == Id && b.IsDeleted == false);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpGet]
        //[Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create(Author author)
        {
            if (!ModelState.IsValid)
            {

                return View(author);
            }

            if (await _context.Authors.AnyAsync(b => b.IsDeleted == false && b.Name.ToLower() == author.Name.Trim().ToLower() && b.Surname.ToLower() == author.Surname.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"{author.Name} {author.Surname} adda Muellif movcuddur");
                return View(author);
            }

            author.Name = author.Name.Trim();
            author.CreatedAt = DateTime.UtcNow.AddHours(4);
            author.CreatedBy = "System";

            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? Id)
        {
            if (Id == null) return BadRequest();
            Author author = await _context.Authors.FirstOrDefaultAsync(b => b.Id == Id && b.IsDeleted == false);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int? Id, Author author)
        {
            if (!ModelState.IsValid) return View(author);
            if (Id == null) return BadRequest();
            if (Id != author.Id) return BadRequest();
            Author dbAuthor = await _context.Authors.FirstOrDefaultAsync(b => b.Id == Id && b.IsDeleted == false);
            if (dbAuthor == null) return NotFound();
            

            dbAuthor.Name = author.Name.Trim();
            dbAuthor.Surname = author.Surname.Trim();
            dbAuthor.UpdatedAt = DateTime.UtcNow.AddHours(4);
            dbAuthor.UpdatedBy = "System";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null) return BadRequest();
            Author author = await _context.Authors
                .Include(b => b.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(b => b.Id == Id && b.IsDeleted == false);
            if (author == null) return NotFound();

            author.IsDeleted = true;
            author.DeletedBy = "System";
            author.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IQueryable<Author> query = _context.Authors
                .Include(b => b.Products)
                .Where(b => b.IsDeleted == false);
            int pageIndex;

            return PartialView("_AuthorIndexPartial", PageNatedList<Author>.Create(query,pageIndex = 1, 3, 3));
        }
    }
}
