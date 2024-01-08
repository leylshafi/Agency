﻿using Agency.Areas.Admin.ViewModels;
using Agency.Data;
using Agency.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agency.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.Include(c=>c.Products).ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if(!ModelState.IsValid) return View();
            bool result =await _context.Categories.AnyAsync(c=>c.Name == categoryVM.Name);
            if(result)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View();
            }

            Category category = new Category()
            {
                Name = categoryVM.Name,
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category existed = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            UpdateCategoryVM vm = new()
            {
                Name = existed.Name,
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM categoryVM)
        {
            if (id <= 0) return BadRequest();
            Category existed = await _context.Categories.FirstOrDefaultAsync(c=>c.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid) return View(categoryVM);
            bool result = await _context.Categories.AnyAsync(c => c.Name == categoryVM.Name && c.Id!=existed.Id);
            if (result)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View();
            }

            existed.Name = categoryVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if(id<= 0) return BadRequest();
            Category category = await _context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
            if (category is null) return NotFound();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            if(id<= 0) return BadRequest();
            Category category= await _context.Categories.Include(c=>c.Products).FirstOrDefaultAsync(category=>category.Id==id);
            if (category is null) return NotFound();
            return View(category);
        }
    }
}
