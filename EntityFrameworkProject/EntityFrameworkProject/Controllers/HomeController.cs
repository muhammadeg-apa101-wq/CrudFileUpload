using EntityFrameworkProject.Data;
using EntityFrameworkProject.Models;
using EntityFrameworkProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //HttpContext.Session.SetString("name", "Mahmud");
            //HttpContext.Response.Cookies.Append("surname", "Rehimli", new CookieOptions() { MaxAge = TimeSpan.FromMinutes(5)});

            //ViewBag.Name = HttpContext.Session.GetString("name");
            //ViewBag.Surname = HttpContext.Request.Cookies["surname"];

            IEnumerable<Slider> sliders = await _context.Sliders.Where(m => !m.IsDeleted).ToListAsync();
            SliderDetail sliderDetail = await _context.SliderDetails.FirstOrDefaultAsync(m => !m.IsDeleted);

            IEnumerable<Product> products = await _context.Products
                .Include(m => m.ProductImages)
                .Include(m => m.Category)
                .Where(m => !m.IsDeleted)
                .Take(4)
                .ToListAsync();

            IEnumerable<Category> categories = await _context.Categories.Where(m => !m.IsDeleted).ToListAsync();

            HomeVM homeVM = new()
            {
                Sliders = sliders,
                SliderDetail = sliderDetail,
                Products = products,
                Categories = categories
            };

            ViewBag.ProductCount = await _context.Products.CountAsync(m => !m.IsDeleted);

            return View(homeVM);
        }

        public async Task<IActionResult> LoadMore(int skip)
        {
            IEnumerable<Product> products = await _context.Products
    .Include(m => m.ProductImages)
    .Include(m => m.Category)
    .Where(m => !m.IsDeleted)
    .Skip(skip)
    .Take(4)
    .ToListAsync();

            return PartialView("_ProductPartial", products);
        }
    }
}
