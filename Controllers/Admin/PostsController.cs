using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal.Data;
using Personal.Helpers;
using Personal.Models;

namespace Personal.Controllers.Admin
{
    public class PostsController : Controller
    {
        private readonly AppDbContext context;
        public PostsController(AppDbContext context)
        {
            this.context = context;

        }
        public async Task<IActionResult> IndexAsync()
        {

            // var posts = await context.Posts.Where(p => p.PostStatus != PostStatus.Trash).ToListAsync(); //avoid trash posts
            var posts = await context.Posts
            .Include(pc => pc.PostCategories)
                .ThenInclude(c => c.Category)
            .Include(pt => pt.PostTags)
                .ThenInclude(t => t.Tag).ToListAsync();
            return View(posts);
        }

        public async Task<IActionResult> CreateAsync()
        {
            ViewData["categories"] = await context.Categories.ToListAsync();
            ViewData["tags"] = await context.Tags.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post post, int[] SelectedCategoryIds, int[] SelectedTagIds)
        {
            post.Slug = "Nepal";
            // tag.Slug = SlugHelper.GenerateSlug(tag.Title);

            //Category
            foreach (var selectedCategoryIds in SelectedCategoryIds)
            {
                post.PostCategories.Add(new PostCategory { CategoryId = selectedCategoryIds });
            }
            //Tag
            foreach (var selectedTagIds in SelectedTagIds)
            {
                post.PostTags.Add(new PostTag { TagId = selectedTagIds });
            }
            if (!ModelState.IsValid)
                return View();
            context.Add(post);
            await context.SaveChangesAsync();
            TempData["message.success"] = "Saved";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditAsync(int id)
        {
            ViewData["categories"] = await context.Categories.ToListAsync();
            ViewData["tags"] = await context.Tags.ToListAsync();
            var post = await context.Posts
                .Include(pc => pc.PostCategories)
                .Include(pt => pt.PostTags)
            .FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                TempData["message.info"] = "Tag not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Post vm, int[] SelectedCategoryIds, int[] SelectedTagIds)
        {
            var post = await context.Posts
            .Include(p => p.PostCategories)
            .Include(p => p.PostTags)
            .FirstOrDefaultAsync(p => p.Id == id);
            //post.Slug = "Nepal";
            post.Title=vm.Title;
            post.Slug = SlugHelper.GenerateSlug(vm.Title);
            post.Content=vm.Content;
            post.PostStatus=vm.PostStatus;

            //For Categories
            //adding unselected Categories in removedCategories that are in database
            var removedCategories = new List<PostCategory>();
            foreach (var postCategory in post.PostCategories)
            {
                if (!SelectedCategoryIds.Contains(postCategory.CategoryId))
                    removedCategories.Add(postCategory);
            }
            //removing old categories
            foreach (var postCategory in removedCategories)
            {
                context.PostCategories.Remove(postCategory);
            }
            //add newly selected categories
            foreach (var selectedCatId in SelectedCategoryIds)
            {
                if (!post.PostCategories.Any(pc => pc.CategoryId == selectedCatId))
                    context.PostCategories.Add(new PostCategory { CategoryId = selectedCatId, PostId = vm.Id });
            }

            //For Tags
            //adding unselected Tags in removedTags that are in database
            var removedTags = new List<PostTag>();
            foreach (var postTags in post.PostTags)
            {
                if (!SelectedTagIds.Contains(postTags.TagId))
                    removedTags.Add(postTags);
            }
            //removing unselected tags
            foreach (var postTags in removedTags)
            {
                post.PostTags.Remove(postTags);
            }
            //adding newly selected Tags
            foreach (var selectedTags in SelectedTagIds)
            {
                if (!post.PostTags.Any(pt => pt.TagId == selectedTags))
                    post.PostTags.Add(new PostTag { TagId = selectedTags, PostId = post.Id });
            }
            if (!ModelState.IsValid)
                return View();
            await context.SaveChangesAsync();
            TempData["message.success"] = "Updated";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null)
            {
                TempData["message.info"] = "Tag not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null)
            {
                TempData["message.info"] = "Tag not Found";
                return RedirectToAction(nameof(Index));
            }
            context.Remove(post);
            await context.SaveChangesAsync();
            TempData["message.success"] = "Removed";
            return RedirectToAction(nameof(Index));
        }

    }
}