using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal.Data;
using Personal.Helpers;
// using Personal.Helpers;
using Personal.Models;

namespace Personal.Controllers.Admin
{
    public class TagsController : Controller
    {
        private readonly AppDbContext context;
        public TagsController(AppDbContext context)
        {
            this.context = context;

        }
        public async Task<IActionResult> Index()
        {
            var tags = await context.Tags.ToListAsync();
            return View(tags);
        }

        public IActionResult Create()
        {
            var tag=new Tag();
            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            
            tag.Slug = SlugHelper.GenerateSlug(tag.Title);
            if (!ModelState.IsValid)
                return View();
            await context.AddAsync(tag);
            await context.SaveChangesAsync();
            TempData["message.success"] = "Saved";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> EditAsync(int id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                TempData["message.info"] = "Tag not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Tag tag)
        {
            tag.Slug = SlugHelper.GenerateSlug(tag.Title);
            if (!ModelState.IsValid)
                return View();
            context.Update(tag);
            await context.SaveChangesAsync();
            TempData["message.success"] = "Updated";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                TempData["message.info"] = "Tag not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null)
            {
                TempData["message.info"] = "Tag not Found";
                return RedirectToAction(nameof(Index));
            }
            context.Remove(tag);
            await context.SaveChangesAsync();
            TempData["message.success"] = "Removed";
            return RedirectToAction(nameof(Index));
        }

    }
}