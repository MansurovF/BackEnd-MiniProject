using Microsoft.AspNetCore.Mvc;
using Pustok_BackEndProject.DataAccessLayer;
using Pustok_BackEndProject.Extensions;
using Pustok_BackEndProject.Helpers;
using Pustok_BackEndProject.Models;
using Pustok_BackEndProject.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace MiniBackEnd.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {

            IQueryable<Product> queries = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Author)
                .Include(p => p.ProductTags.Where(pt => pt.IsDeleted == false)).ThenInclude(p => p.Tag)
                .Where(p => p.IsDeleted == false);


            return View(PageNatedList<Product>.Create(queries, pageIndex, 3, 5));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();



            if (!ModelState.IsValid)
            {
                return View(product);
            }
            if (product.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Categoriya Mutleq secilmelidir!");
                return View(product);
            }
            if (product.BrandId == null)
            {
                ModelState.AddModelError("BrandId", "Brend mutleq secilmelidir!");
                return View(product);
            }
            if (product.AuthorId == null)
            {
                ModelState.AddModelError("AuthorId", "Author mutleq secilmelidir!");
                return View(product);
            }
            if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Duzgun Categoriya secin!");
                return View(product);
            }
            if (!await _context.Brands.AnyAsync(c => c.IsDeleted == false && c.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Duzgun brend secin!");
                return View(product);
            }
            if (!await _context.Authors.AnyAsync(c => c.IsDeleted == false && c.Id == product.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Duzgun Author secin!");
                return View(product);
            }

            if (product.MainFile != null)
            {
                if (product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli shekil novu duzgun deyil");
                    return View(product);
                }
                if (product.MainFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli shekil olcusu coxdur!");
                    return View(product);
                }

                product.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "image", "products");
            }
            else
            {
                ModelState.AddModelError("MainFile", "MainFile mutleqdir!");
                return View(product);
            }

            if (product.HoverFile != null)
            {
                if (product.HoverFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli shekil novu duzgun deyil");
                    return View(product);
                }
                if (product.HoverFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli shekil olcusu coxdur!");
                    return View(product);
                }

                product.HoverImage = await product.HoverFile.CreateFileAsync(_env, "assets", "image", "products");
            }
            else
            {
                ModelState.AddModelError("HoverFile", "HoverFile mutleqdir!");
                return View(product);
            }

            //Many to Many
            if (product.TagIds != null && product.TagIds.Count() > 0)
            {
                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.IsDeleted == false && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"{tagId} deyeri yalnisdir!");
                        return View(product);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };
                    productTags.Add(productTag);

                }
                product.ProductTags = productTags;
            }

            if (product.Files != null & product.Files.Count() > 6)
            {
                ModelState.AddModelError("Files", "File sayi coxdur! 6 dan cox fayl gondermek olmaz!");
                return View(product);
            }


            //MultiFile Upload
            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (IFormFile file in product.Files)
                {
                    if (file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli shekil novu duzgun deyil");
                        return View(product);
                    }
                    if (file.CheckFileLenght(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli shekil olcusu coxdur!");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env, "assets", "image", "products"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);


                }
                product.ProductImages = productImages;
            }
            else
            {
                ModelState.AddModelError("Files", "Sekil mutleq secilmelidir!");
                return View(product);
            }

            string seria = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId).Name.Substring(0, 2);
            seria += _context.Brands.FirstOrDefault(c => c.Id == product.BrandId).Name.Substring(0, 2);
            seria += _context.Authors.FirstOrDefault(c => c.Id == product.AuthorId).Name.Substring(0, 2);
            seria = seria.ToLower();

            int code = _context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault() != null ?
                (int)_context.Products.Where(p => p.Seria == seria).OrderByDescending(p => p.Id).FirstOrDefault().Code + 1 : 1;

            product.Seria = seria;
            product.Code = code;
            product.CreatedAt = DateTime.UtcNow.AddHours(4);
            product.CreatedBy = "System";



            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {

            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products
                .Include(p => p.ProductTags.Where(pt => pt.IsDeleted == false))
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (product == null) return NotFound();

            ViewBag.Authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            product.TagIds = product.ProductTags?.Select(x => x.TagId);


            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteImage(int? id, int? imageId)
        {

            if (id == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) return NotFound();

            if (!product.ProductImages.Any(pi => pi.Id == imageId)) return BadRequest();

            if (product.ProductImages.Count() <= 1)
            {
                return BadRequest();
            }


            product.ProductImages.FirstOrDefault(p => p.Id == imageId).IsDeleted = true;
            product.ProductImages.FirstOrDefault(p => p.Id == imageId).DeletedBy = "System";
            product.ProductImages.FirstOrDefault(p => p.Id == imageId).DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            FileHelper.DeleteFile(product.ProductImages.FirstOrDefault(p => p.Id == imageId).Image, _env, "assets", "image", "products");



            return PartialView("_ProductImagePartial", product.ProductImages.Where(pi => pi.IsDeleted == false).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Authors = await _context.Authors.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (id == null)
            {
                return BadRequest();
            }
            if (id != product.Id) return BadRequest();

            Product dbproduct = await _context.Products
                .Include(p => p.ProductTags.Where(pt => pt.IsDeleted == false))
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (dbproduct == null) return NotFound();

            if (product.CategoryId == null)
            {
                ModelState.AddModelError("CategoryId", "Kateqoriya Mutleq secilmelidir!");
                return View(product);
            }
            if (product.BrandId == null)
            {
                ModelState.AddModelError("BrandId", "Brend mutleq secilmelidir!");
                return View(product);
            }
            if (product.AuthorId == null)
            {
                ModelState.AddModelError("AuthorId", "Author mutleq secilmelidir!");
                return View(product);
            }
            if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Duzgun kateqoriya secin!");
                return View(product);
            }
            if (!await _context.Brands.AnyAsync(c => c.IsDeleted == false && c.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Duzgun brend secin!");
                return View(product);
            }
            if (!await _context.Authors.AnyAsync(c => c.IsDeleted == false && c.Id == product.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Duzgun author secin!");
                return View(product);
            }

            if (product.MainFile != null)
            {
                if (product.MainFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli shekil novu duzgun deyil");
                    return View(product);
                }
                if (product.MainFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("MainFile", $"{product.MainFile.FileName} adli shekil olcusu coxdur!");
                    return View(product);
                }

                FileHelper.DeleteFile(dbproduct.MainImage, _env, "assets", "image", "products");


                dbproduct.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "image", "products");
            }

            if (product.HoverFile != null)
            {
                if (product.HoverFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli shekil novu duzgun deyil");
                    return View(product);
                }
                if (product.HoverFile.CheckFileLenght(300))
                {
                    ModelState.AddModelError("HoverFile", $"{product.HoverFile.FileName} adli shekil olcusu coxdur!");
                    return View(product);
                }

                FileHelper.DeleteFile(dbproduct.HoverImage, _env, "assets", "image", "products");


                dbproduct.HoverImage = await product.HoverFile.CreateFileAsync(_env, "assets", "image", "products");
            }

            if (product.TagIds != null && product.TagIds.Count() > 0)
            {
                _context.ProductTags.RemoveRange(dbproduct.ProductTags);

                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => t.IsDeleted == false && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"{tagId} deyeri yalnisdir!");
                        return View(product);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId,
                        ProductId = product.Id,
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };
                    productTags.Add(productTag);

                }
                
                dbproduct.ProductTags.AddRange(productTags);
            }

            int canUpload = 6 - (dbproduct.ProductImages != null ? dbproduct.ProductImages.Count() : 0);

            if (product.Files != null && canUpload < product.Files.Count())
            {
                ModelState.AddModelError("Files", $"Maksimum {canUpload} shekil yukleye bilersiz!");
                dbproduct.TagIds = product.TagIds;
                return View(dbproduct);
            }

            if (product.Files != null && product.Files.Count() > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();
                foreach (IFormFile file in product.Files)
                {
                    if (file.CheckFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli shekil novu duzgun deyil");
                        return View(product);
                    }
                    if (file.CheckFileLenght(300))
                    {
                        ModelState.AddModelError("Files", $"{file.FileName} adli shekil olcusu coxdur!");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env, "assets", "image", "products"),
                        CreatedAt = DateTime.UtcNow.AddHours(4),
                        CreatedBy = "System"
                    };

                    productImages.Add(productImage);


                }
                dbproduct.ProductImages.AddRange(productImages);
            }

            dbproduct.Title = product.Title;
            dbproduct.MainDescription = product.MainDescription;
            dbproduct.Price = product.Price;
            dbproduct.DiscountedPrice = product.DiscountedPrice;
            dbproduct.Extax = product.Extax;
            dbproduct.Count = product.Count;
            dbproduct.IsMostviewProducts = product.IsMostviewProducts;
            dbproduct.IsFeatured = product.IsFeatured;
            dbproduct.IsNewArrival = product.IsNewArrival;
            dbproduct.UpdatedBy = "System";
            dbproduct.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.Id == id)) return BadRequest();

            Product dbproduct = await _context.Products
                .Include(p => p.ProductTags.Where(pt => pt.IsDeleted == false))
                .Include(p => p.ProductImages.Where(pi => pi.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            if (dbproduct == null)
            {
                return NotFound();
            }

            dbproduct.IsDeleted = true;

            await _context.SaveChangesAsync();


            IQueryable<Product> queries = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Author)
                .Include(p => p.ProductTags.Where(pt => pt.IsDeleted == false)).ThenInclude(p => p.Tag)
                .Where(p => p.IsDeleted == false);

            int pageIndex;

            return PartialView("_ProductIndexPartial", PageNatedList<Product>.Create(queries, pageIndex = 1, 3, 5));
        }
    }
}
