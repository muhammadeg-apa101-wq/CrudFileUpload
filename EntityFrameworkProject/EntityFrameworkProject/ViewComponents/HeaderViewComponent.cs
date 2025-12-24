using System.Linq;
using EntityFrameworkProject.Data;
using EntityFrameworkProject.Models;
using EntityFrameworkProject.Services;
using EntityFrameworkProject.ViewModels;
using EntityFrameworkProject.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EntityFrameworkProject.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly LayoutService _layoutService;
        public HeaderViewComponent(LayoutService layoutService)
        {
            _layoutService = layoutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basketCookie = HttpContext.Request.Cookies["basket"];
            int count = 0;
            decimal price = 0m;

            if (!string.IsNullOrWhiteSpace(basketCookie))
            {
                var basket = JsonConvert.DeserializeObject<List<BasketVM>>(basketCookie) ?? new List<BasketVM>();
                count = basket.Sum(b => b.Count);
                price = basket.Sum(b => b.Count * b.Price);
            }

            Dictionary<string, string> settings = await _layoutService.GetSettings();
            HeaderVM headerVM = new HeaderVM
            {
                Settings = settings,
                BasketCount = count,
                BasketPrice = price
            };

            return await Task.FromResult(View(headerVM));
        }
    }
}
