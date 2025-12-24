using EntityFrameworkProject.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SliderDetail> SliderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<Blog>().HasData(
             new Blog 
             {
                 Id = 1,
                 Image = "blog-feature-img-1.jpg",
                Title = "Boost your conversion rate",
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc ut.",
                CreatedAt = new DateTime(2025, 12, 22),
                IsDeleted = false
             },
                new Blog 
                {
                    Id = 2,
                    Image = "blog-feature-img-3.jpg",
                    Title = "Boost your salam rate",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc ut.",
                    CreatedAt = new DateTime(2025, 12, 22),
                    IsDeleted = false
                },
                new Blog 
                {
                    Id = 3,
                    Image = "blog-feature-img-4.jpg",
                    Title = "Boost your sagol rate",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed euismod, nunc ut.",
                    CreatedAt = new DateTime(2025, 12, 22),
                    IsDeleted = false
                }
                );
        }
    }
}
