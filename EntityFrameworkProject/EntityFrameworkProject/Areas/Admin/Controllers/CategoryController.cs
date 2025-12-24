using EntityFrameworkProject.Areas.Admin.ViewModels.CategoryVMs;
using EntityFrameworkProject.Data;
using EntityFrameworkProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _context.Categories
                .OrderByDescending(m => m.Id)
                .Where(m => !m.IsDeleted)
                .ToListAsync();

            IEnumerable<GetAllCategoryVM> getAllCategoryVMs = categories.Select(c => new GetAllCategoryVM
            {
                Id = c.Id,
                Name = c.Name
            });

            return View(getAllCategoryVMs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryVM request)
        {
            if (!ModelState.IsValid) return View(request);

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
                return View(request);
            }


            string newName = request.Name.Trim();


            bool isExist = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == newName.ToLower());

            if (isExist)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View(request);
            }

            Category newCategory = new()
            {
                Name = newName
            };

            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            Category? category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null) return NotFound();
            DetailCategoryVM detailCategoryVM = new()
            {
                Id = category.Id,
                Name = category.Name
            };
            return View(detailCategoryVM);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Category? category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null) return NotFound();

            UpdateCategoryVM updateCategoryVM = new()
            {
                Name = category.Name
            };

            return View(updateCategoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM request)
        {
            if (!ModelState.IsValid) return View(request);
            Category? category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null) return NotFound();
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
                return View(request);
            }
            string newName = request.Name.Trim();
            bool isExist = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == newName.ToLower() && c.Id != id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View(request);
            }
            category.Name = newName;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Category? category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (category == null) return NotFound();
            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
