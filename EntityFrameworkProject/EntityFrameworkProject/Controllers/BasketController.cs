using EntityFrameworkProject.Data;
using EntityFrameworkProject.Models;
using EntityFrameworkProject.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EntityFrameworkProject.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var cookie = HttpContext.Request.Cookies["basket"];
            List<BasketVM> basket;
            if (string.IsNullOrWhiteSpace(cookie))
            {
                basket = new List<BasketVM>();
            }
            else
            {
                try
                {
                    basket = JsonConvert.DeserializeObject<List<BasketVM>>(cookie) ?? new List<BasketVM>();
                }
                catch (JsonException)
                {

                    basket = new List<BasketVM>();
                }
            }

            List<BasketDetailVM> basketDetails = new();

            foreach (var item in basket)
            {
                if (item == null) continue;

                var product = await _context.Products
                    .Include(m => m.ProductImages)
                    .Include(m => m.Category)
                    .FirstOrDefaultAsync(m => m.Id == item.Id && !m.IsDeleted);


                if (product == null) continue;

                basketDetails.Add(new BasketDetailVM
                {
                    Id = item.Id,
                    Count = item.Count,
                    Image = product.ProductImages.FirstOrDefault(m => m.IsMain)?.Image,
                    Name = product.Name ?? string.Empty,
                    Category = product.Category?.Name ?? string.Empty,
                    Price = product.Price,
                    TotalPrice = product.Price * item.Count
                });
            }

            return View(basketDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int? id)
        {
            if (id is null) return BadRequest();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (product == null) return NotFound();

            List<BasketVM> basket;

            var cookie = HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(cookie))
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(cookie) ?? new();
            }
            else
            {
                basket = new();
            }

            var isProductExist = basket.FirstOrDefault(m => m.Id == id);

            if (isProductExist == null)
            {
                basket.Add(new BasketVM
                {
                    Id = id.Value,
                    Count = 1,
                    Price = product.Price
                });
            }
            else
            {
                isProductExist.Count++;
            }

            HttpContext.Response.Cookies.Append(
                "basket",
                JsonConvert.SerializeObject(basket),
                new CookieOptions { HttpOnly = true }
            );

            return Ok(new
            {
                count = basket.Sum(m => m.Count),
                totalPrice = basket.Sum(m => m.Count * m.Price)
            });
        }
    }
}