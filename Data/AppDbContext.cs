using Microsoft.EntityFrameworkCore;
using Personal.Models;

namespace Personal.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Entity<PostTag>().HasKey(pt=>new{pt.PostId,pt.TagId});
            mb.Entity<PostCategory>().HasKey(pc=>new{pc.PostId,pc.CategoryId});

            mb.Entity<Post>()
            .Property(p=>p.Title).IsRequired();
             mb.Entity<Post>()
            .Property(p=>p.Slug).IsRequired();
            mb.Entity<Tag>()
            .Property(t=>t.Title).IsRequired();
             mb.Entity<Tag>()
            .Property(t=>t.Slug).IsRequired();
            mb.Entity<Category>()
            .Property(c=>c.Title).IsRequired();
             mb.Entity<Category>()
            .Property(c=>c.Slug).IsRequired();

        }
    }
}