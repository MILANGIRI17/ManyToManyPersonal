using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal.Data;
using Personal.Helpers;
// using Personal.Helpers;
using Personal.Models;

namespace Personal.Controllers.Admin
{
    public class CategoriesController : Controller
    {
        private readonly AppDbContext context;
        public CategoriesController(AppDbContext context)
        {
            this.context = context;

        }
        public async Task<IActionResult> Index()
        {
            var categories = await context.Categories.ToListAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            category.Slug = SlugHelper.GenerateSlug(category.Title);
            if (!ModelState.IsValid)
                return View();
            await context.AddAsync(category);
            await context.SaveChangesAsync();
            TempData["message.success"] = "Saved";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> EditAsync(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["message.info"] = "Category not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            category.Slug = SlugHelper.GenerateSlug(category.Title);
            if (!ModelState.IsValid)
                return View();
            context.Update(category);
            await context.SaveChangesAsync();
            TempData["message.success"] = "Updated";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["message.info"] = "Category not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["message.info"] = "Category not Found";
                return RedirectToAction(nameof(Index));
            }
            context.Remove(category);
            await context.SaveChangesAsync();
            TempData["message.success"] = "Removed";
            return RedirectToAction(nameof(Index));
        }

    }
}