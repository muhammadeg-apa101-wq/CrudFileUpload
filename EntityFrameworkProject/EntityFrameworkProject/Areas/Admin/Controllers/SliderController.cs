using EntityFrameworkProject.Areas.Admin.ViewModels.CategoryVMs;
using EntityFrameworkProject.Areas.Admin.ViewModels.SliderVMs;
using EntityFrameworkProject.Data;
using EntityFrameworkProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await _context.Sliders.Where(m => !m.IsDeleted).ToListAsync();

            IEnumerable<GetAllSliderVM> getAllSliderVMs = sliders.Select(c => new GetAllSliderVM
            {
                Id = c.Id,
                Image = c.Image
            });

            return View(getAllSliderVMs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSliderVm request)
        {
            if (!ModelState.IsValid) return View(request);

            string fileName = Guid.NewGuid().ToString() + "_" + request.Photo.FileName;
            string path = Path.Combine(_env.WebRootPath, "img", fileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await request.Photo.CopyToAsync(stream);
            }

            Slider slider = new Slider
            {
                Image = fileName
            };

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
