using Agency.Areas.Admin.ViewModels;
using Agency.Data;
using Agency.Models;
using Agency.Utilities.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agency.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(int page)
        {
            double count = await _context.Products.CountAsync();
            var products = await _context.Products.Skip(page*3).Take(3).Include(p=>p.Category).ToListAsync();
            PaginationVM<Product> paginationVM = new()
            {
                CurrentPage = page,
                Items = products,
                TotalPage = Math.Ceiling(count / 3)
            };
            return View(paginationVM);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM vm = new CreateProductVM()
            {
                Categories = await _context.Categories.ToListAsync()
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                return View(productVM);
            }

            if (!productVM.Photo.ValidateType())
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("Photo", "File type is invalid");
            }
            if (!productVM.Photo.ValidateSize(2))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("Photo", "File size is invalid");
            }

            Product product = new()
            {
                Name = productVM.Name,
                CategoryId = productVM.CategoryId,
                ImageUrl = await productVM.Photo.CreateFile(_env.WebRootPath, "assets", "img")
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product existed = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (existed == null) return NotFound();
            UpdateProductVM vm = new()
            {
                Name= existed.Name,
                CategoryId=existed.CategoryId,
                ImageUrl= existed.ImageUrl,
                Categories = await _context.Categories.ToListAsync(),
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            if(id <= 0) return BadRequest();
            Product existed = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) return NotFound();
            if (productVM.Photo is not null)
            {
                if (!productVM.Photo.ValidateType())
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    ModelState.AddModelError("Photo", "File type is invalid");
                }
                if (!productVM.Photo.ValidateSize(2))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    ModelState.AddModelError("Photo", "File size is invalid");
                }
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
                existed.ImageUrl = await productVM.Photo.CreateFile(_env.WebRootPath, "assets", "img");
            }
            existed.Name = productVM.Name;
            existed.CategoryId = productVM.CategoryId;
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return BadRequest();
            Product existed = await _context.Products.FirstOrDefaultAsync(p=>p.Id == id);   
            if (existed == null) return NotFound();
            existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "img");
            _context.Products.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            if(id<=0) return BadRequest();
            Product existed = await _context.Products.Include(p=>p.Category).FirstOrDefaultAsync(p=>p.Id==id);
            if (existed == null) return NotFound();
            return View(existed);
        }

             
    }
}
