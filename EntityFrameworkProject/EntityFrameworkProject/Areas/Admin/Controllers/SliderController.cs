using EntityFrameworkProject.Areas.Admin.ViewModels.CategoryVMs;
using EntityFrameworkProject.Areas.Admin.ViewModels.SliderVMs;
using EntityFrameworkProject.Data;
using EntityFrameworkProject.Helpers;
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

            if (!request.Photo.CheckFileType("image/"))
            {
                ModelState.AddModelError("Photo", "File type must be image");
                return View(request);
            }

            if (!request.Photo.CheckFileSize(200))
            {
                ModelState.AddModelError("Photo", "Image size must be max 200kb");
                return View(request);
            }

            string fileName = request.Photo.GenerateFileName();
            string path = _env.WebRootPath.GetFilePath("img", fileName);

            request.Photo.SaveFile(path);

            Slider newSlider = new Slider
            {
                Image = fileName
            };

            await _context.Sliders.AddAsync(newSlider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Slider? slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
            if (slider == null) return NotFound();
            string path = _env.WebRootPath.GetFilePath("img", slider.Image);

            path.DeleteFile();

            _context.Sliders.Remove(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            Slider? slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
            if (slider == null) return NotFound();
            UpdateSliderVM updateSliderVM = new UpdateSliderVM
            {
                Image = slider.Image
            };
            return View(updateSliderVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSliderVM request)
        {
            if (!ModelState.IsValid) return View(request);
            Slider? slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
            if (slider == null) return NotFound();
            if (request.Photo != null)
            {
                if (!request.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View(request);
                }
                if (!request.Photo.CheckFileSize(200))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 200kb");
                    return View(request);
                }
                string oldPath = _env.WebRootPath.GetFilePath("img", slider.Image);
                oldPath.DeleteFile();
                string fileName = request.Photo.GenerateFileName();
                string newPath = _env.WebRootPath.GetFilePath("img", fileName);
                request.Photo.SaveFile(newPath);
                slider.Image = fileName;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if(id == null) return NotFound();


            Slider? slider = await _context.Sliders.FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
            if (slider == null) return NotFound();
            DetailSliderVM detailSliderVM = new DetailSliderVM
            {
                Image = slider.Image
            };
            return View(detailSliderVM);
        }
    }
}
